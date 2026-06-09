namespace Aesys.Core.Shared.ContactForm;

// Model for the _Success partial. Carries only the section theme so the
// thank-you message styles itself to match the surface it replaces the form on.
// A typed model (rather than threading theme through ViewData) keeps the partial
// strongly-typed like every other view in the component.
public sealed record ContactSuccessViewModel(string Theme);
