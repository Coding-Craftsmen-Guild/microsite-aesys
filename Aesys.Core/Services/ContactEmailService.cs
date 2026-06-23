using System.Text;
using Aesys.Core.Localization;
using Aesys.Core.Shared.ContactForm;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;

namespace Aesys.Core.Services;

// Sends a validated contact submission to the block's configured recipients via
// Umbraco's IEmailSender (which wraps the configured SMTP server — see
// Umbraco:CMS:Global:Smtp in appsettings). Keeps the controller thin: the
// controller validates and maps, this owns the message build + delivery.
public interface IContactEmailService
{
    // True only when an SMTP host is configured. The controller checks this before
    // attempting a send so a misconfigured environment surfaces a clear error
    // instead of a swallowed exception.
    bool CanSend { get; }

    Task SendAsync(ContactFormSubmission submission, CancellationToken cancellationToken = default);
}

public sealed class ContactEmailService(
    IEmailSender emailSender,
    ILocalizer localizer,
    ILogger<ContactEmailService> logger
) : IContactEmailService
{
    private const string EmailType = "ContactForm";

    public bool CanSend => emailSender.CanSendRequiredEmail();

    public async Task SendAsync(
        ContactFormSubmission submission,
        CancellationToken cancellationToken = default
    )
    {
        var recipients = ParseRecipients(submission.Recipients);
        if (recipients.Length == 0)
        {
            // No destination configured on the block — a content/config mistake.
            // Throw so the controller returns a server error rather than silently
            // "succeeding" with mail going nowhere.
            throw new InvalidOperationException(
                "Contact form has no recipients configured; cannot send submission."
            );
        }

        var subject = $"{localizer["Email.SubjectPrefix"]} {submission.Name}";
        var body = BuildBody(submission);

        // from = null: IEmailSender falls back to the configured From address.
        // replyTo = the visitor's email so a reply from the inbox goes to them.
        var message = new EmailMessage(
            from: null,
            to: recipients,
            cc: null,
            bcc: null,
            replyTo: string.IsNullOrWhiteSpace(submission.Email) ? null : [submission.Email],
            subject: subject,
            body: body,
            isBodyHtml: true,
            attachments: null
        );

        logger.LogInformation(
            "Sending contact form submission from {Email} to {RecipientCount} recipient(s).",
            submission.Email,
            recipients.Length
        );

        // enableNotification: false (this isn't an Umbraco notification email);
        // expires: null (no scheduled-send window). The 2-arg overload is obsolete
        // in v17 — this is the supported signature.
        await emailSender.SendAsync(message, EmailType, false, null);
    }

    // Split a comma/semicolon-separated recipients string into trimmed, non-empty
    // addresses. Whitespace and stray separators are dropped.
    private static string[] ParseRecipients(string recipients) =>
        string.IsNullOrWhiteSpace(recipients)
            ? []
            : recipients
                .Split(
                    [',', ';'],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                )
                .ToArray();

    private string BuildBody(ContactFormSubmission s)
    {
        var sb = new StringBuilder();
        sb.Append($"<h2>{HtmlEncode(localizer["Email.Heading"])}</h2>");
        sb.Append("<table cellpadding=\"6\" style=\"border-collapse:collapse\">");
        AppendRow(sb, localizer["Email.NameLabel"], s.Name);
        AppendRow(sb, localizer["Email.CompanyLabel"], s.Company);
        AppendRow(sb, localizer["Email.EmailLabel"], s.Email);
        AppendRow(sb, localizer["Email.PhoneLabel"], s.Phone);
        sb.Append("</table>");
        sb.Append($"<h3>{HtmlEncode(localizer["Email.MessageHeading"])}</h3>");
        sb.Append($"<p>{HtmlEncode(s.Message).Replace("\n", "<br/>")}</p>");
        return sb.ToString();
    }

    private static void AppendRow(StringBuilder sb, string label, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        sb.Append("<tr>");
        sb.Append($"<td style=\"font-weight:bold;vertical-align:top\">{HtmlEncode(label)}</td>");
        sb.Append($"<td>{HtmlEncode(value)}</td>");
        sb.Append("</tr>");
    }

    private static string HtmlEncode(string value) =>
        System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
}
