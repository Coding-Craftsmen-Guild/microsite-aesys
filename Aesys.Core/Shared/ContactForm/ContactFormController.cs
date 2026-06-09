using Aesys.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace Aesys.Core.Shared.ContactForm;

// Submit endpoint for the Contact Form block. Routed as a surface controller at
// /umbraco/surface/ContactForm/Submit. Mirrors the BlogListing controller's
// "thin HTTP seam returning a partial" shape, but for a POST:
//
//   - Invalid ModelState  -> re-render the _Form partial WITH the posted values
//     and validation messages, returned as 422 so the client swaps it back in
//     and shows the errors inline.
//   - Valid + mail sent    -> render the _Success partial; the client swaps the
//     whole form out for it.
//
// Antiforgery is enforced (SurfaceController POST validates the token by default);
// the form renders @Html.AntiForgeryToken() and the client posts the field.
public sealed class ContactFormController(
    IUmbracoContextAccessor umbracoContextAccessor,
    IUmbracoDatabaseFactory databaseFactory,
    ServiceContext services,
    AppCaches appCaches,
    IProfilingLogger profilingLogger,
    IPublishedUrlProvider publishedUrlProvider,
    IContactEmailService contactEmail,
    ILogger<ContactFormController> logger
)
    : SurfaceController(
        umbracoContextAccessor,
        databaseFactory,
        services,
        appCaches,
        profilingLogger,
        publishedUrlProvider
    )
{
    [HttpPost]
    public async Task<IActionResult> Submit([Bind(Prefix = "")] ContactFormSubmission model)
    {
        // Normalize the section theme the POST carried as a hidden field, so the
        // re-rendered form (_Form reads Model.Theme) and the success message match
        // the surface they live on. Default to light if absent.
        model.Theme = string.IsNullOrWhiteSpace(model.Theme) ? "light" : model.Theme;

        if (!ModelState.IsValid)
        {
            Response.StatusCode = UnprocessableEntityStatus;
            return PartialView("Components/ContactForm/_Form", model);
        }

        try
        {
            if (!contactEmail.CanSend)
            {
                logger.LogError(
                    "Contact form submission received but no SMTP server is configured."
                );
                return MailFailure(model);
            }

            await contactEmail.SendAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send contact form submission.");
            return MailFailure(model);
        }

        return PartialView(
            "Components/ContactForm/_Success",
            new ContactSuccessViewModel(model.Theme)
        );
    }

    // Re-render the form with a form-level error and a 422 so the client keeps the
    // visitor's input and shows the failure without losing what they typed.
    private IActionResult MailFailure(ContactFormSubmission model)
    {
        ModelState.AddModelError(
            string.Empty,
            "Slanje poruke trenutno nije moguće. Molimo pokušajte ponovo kasnije."
        );
        Response.StatusCode = UnprocessableEntityStatus;
        return PartialView("Components/ContactForm/_Form", model);
    }

    // 422 Unprocessable Entity. The signal contract with contact-form.ts: any
    // 422 means "swap the body back in (form with errors)"; 200 means "success".
    // Spelled as a const so we don't pull Microsoft.AspNetCore.Http.StatusCodes
    // into Aesys.Core just for one value. Named ...Status to avoid colliding with
    // ControllerBase.UnprocessableEntity().
    private const int UnprocessableEntityStatus = 422;
}
