using System.ComponentModel.DataAnnotations;

namespace Aesys.Core.Shared.ContactForm;

// The posted contact form. DataAnnotations drive both ModelState (server) and the
// markup hints; the controller re-renders the form partial with these errors when
// invalid. Display names are the Serbian field labels so validation summaries read
// in the site language. `Recipients` is a hidden, server-trusted field carried from
// the block config — never shown to the visitor — so the controller knows where to
// send without trusting a client-supplied address.
public sealed class ContactFormSubmission
{
    [Display(Name = "Ime i prezime")]
    [Required(ErrorMessage = "Ime i prezime je obavezno.")]
    [StringLength(150, ErrorMessage = "Ime i prezime ne sme biti duže od 150 karaktera.")]
    public string Name { get; set; }

    [Display(Name = "Kompanija")]
    [StringLength(150, ErrorMessage = "Naziv kompanije ne sme biti duži od 150 karaktera.")]
    public string Company { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
    [StringLength(254, ErrorMessage = "Email adresa je predugačka.")]
    public string Email { get; set; }

    [Display(Name = "Telefon")]
    [Required(ErrorMessage = "Telefon je obavezan.")]
    [Phone(ErrorMessage = "Unesite ispravan broj telefona.")]
    [StringLength(40, ErrorMessage = "Broj telefona je predugačak.")]
    public string Phone { get; set; }

    [Display(Name = "Poruka")]
    [Required(ErrorMessage = "Poruka je obavezna.")]
    [StringLength(4000, ErrorMessage = "Poruka ne sme biti duža od 4000 karaktera.")]
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
