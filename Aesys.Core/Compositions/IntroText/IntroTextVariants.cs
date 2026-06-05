namespace Aesys.Core.Compositions.IntroText;

public static class IntroTextVariants
{
    // Theme drives every colour in the block so the same content reads correctly
    // on a white section or a dark/image section. `<em>` in the title always
    // resolves to the brand green via [&_em] regardless of theme.
    public static string Title(string theme) =>
        theme == "dark"
            ? "text-white [&_em]:not-italic [&_em]:text-accent-400"
            : "text-aesys-800 [&_em]:not-italic [&_em]:text-accent-500";

    // Richtext body: muted relative to the title, still legible per theme.
    public static string Text(string theme) => theme == "dark" ? "text-white/80" : "text-aesys-600";

    // The CTA style follows the theme: solid accent on light, outline-on-dark
    // (white border / white label) on dark so it doesn't fight the background.
    public static string ButtonVariant(string theme) => theme == "dark" ? "lang" : "primary";
}
