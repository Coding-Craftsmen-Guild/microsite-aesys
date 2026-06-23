using System.ComponentModel.DataAnnotations;

namespace Aesys.Core.Shared.ContactForm;

// The posted contact form. DataAnnotations drive both ModelState (server) and the
// markup hints; the controller re-renders the form partial with these errors when
// invalid. Display names and ErrorMessages are dictionary ItemKeys — the
// DataAnnotationLocalizerProvider (see RegisterCore) resolves them against the
// Umbraco dictionary for the current culture, so validation summaries and field
// names read in whatever language the page is. `Recipients` is a hidden,
// server-trusted field carried from the block config — never shown to the visitor —
// so the controller knows where to send without trusting a client-supplied address.
public sealed class ContactFormSubmission
{
    [Display(Name = "ContactForm.NameLabel")]
    [Required(ErrorMessage = "ContactForm.NameRequired")]
    [StringLength(150, ErrorMessage = "ContactForm.NameTooLong")]
    public string Name { get; set; }

    [Display(Name = "ContactForm.CompanyLabel")]
    [StringLength(150, ErrorMessage = "ContactForm.CompanyTooLong")]
    public string Company { get; set; }

    [Display(Name = "ContactForm.EmailLabel")]
    [Required(ErrorMessage = "ContactForm.EmailRequired")]
    [EmailAddress(ErrorMessage = "ContactForm.EmailInvalid")]
    [StringLength(254, ErrorMessage = "ContactForm.EmailTooLong")]
    public string Email { get; set; }

    [Display(Name = "ContactForm.PhoneLabel")]
    [Required(ErrorMessage = "ContactForm.PhoneRequired")]
    [Phone(ErrorMessage = "ContactForm.PhoneInvalid")]
    [StringLength(40, ErrorMessage = "ContactForm.PhoneTooLong")]
    public string Phone { get; set; }

    [Display(Name = "ContactForm.MessageLabel")]
    [Required(ErrorMessage = "ContactForm.MessageRequired")]
    [StringLength(4000, ErrorMessage = "ContactForm.MessageTooLong")]
    public string Message { get; set; }

    // Server-trusted: the block's Recipients property, round-tripped through a
    // hidden field so the POST knows its destination. Validated to be non-empty in
    // the controller (not via [Required], since an empty/forged value is a config
    // problem, not a user-facing validation error).
    public string Recipients { get; set; }

    // Carries the section theme ("dark"/"light") from the initial render through
    // the POST so the controller's re-rendered partial styles match the section it
    // lives in. Presentation-only; not validated.
    public string Theme { get; set; }
}
