namespace Aesys.Core.Shared.ContactForm;

public static class ContactFormVariants
{
    // The form card surface. Mirrors the Cards block: rounded, padded, navy-on-dark
    // / pale-on-light so it lifts off whichever section surface it sits on.
    public const string Card = "rounded-xl p-6 md:p-8";

    public static string CardFill(string theme) =>
        theme == "dark"
            ? "bg-aesys-800 text-white"
            : "bg-surface-light text-aesys-800 border border-aesys-100";

    // Field label.
    public static string Label(string theme) =>
        theme == "dark" ? "text-white/80" : "text-aesys-800/80";

    // Text/email/tel/textarea input chrome. Literal classes so Tailwind's source
    // scan picks every variant up. The invalid ring is toggled by aria-invalid in
    // the markup, not here.
    public static string Field(string theme) =>
        theme == "dark"
            ? "w-full rounded-md border border-white/20 bg-white/5 px-4 py-3 text-base text-white placeholder:text-white/40 outline-none transition focus:border-accent-500 focus:ring-2 focus:ring-accent-500/40 aria-[invalid=true]:border-red-400 aria-[invalid=true]:ring-red-400/40"
            : "w-full rounded-md border border-aesys-100 bg-white px-4 py-3 text-base text-aesys-800 placeholder:text-aesys-800/40 outline-none transition focus:border-accent-500 focus:ring-2 focus:ring-accent-500/40 aria-[invalid=true]:border-red-500 aria-[invalid=true]:ring-red-500/40";

    // Inline per-field validation message.
    public const string FieldError = "mt-1 block text-base text-red-500";

    // Form-level error banner (mail send failure / summary).
    public static string Summary(string theme) =>
        theme == "dark"
            ? "rounded-md border border-red-400/40 bg-red-500/10 px-4 py-3 text-base text-red-200"
            : "rounded-md border border-red-500/30 bg-red-50 px-4 py-3 text-base text-red-700";

    // The submit button. Secondary (outline) variant of the Button component —
    // white pill, 1px accent border, navy label — applied to a real
    // <button type="submit"> (the Button component renders an <a>, so we mirror
    // its outline classes here instead of invoking it).
    public const string Submit =
        "inline-flex h-control items-center justify-center rounded-full border border-accent-500 bg-white px-8 text-base font-medium text-aesys-800 transition hover:bg-accent-50 disabled:cursor-not-allowed disabled:opacity-60";
}
