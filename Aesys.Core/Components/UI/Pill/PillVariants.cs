namespace Aesys.Core.Components.UI.Pill;

public static class PillVariants
{
    // Small full-radius label chip. Geometry mirrors the lang/tag pills in
    // ButtonVariants (h-control-sm). Colour comes from Theme().
    public const string Base =
        "inline-flex h-control-sm items-center rounded-full border px-4 text-base leading-none";

    // light — on white/light sections: brand navy outline + navy label.
    // dark  — on dark/image sections: translucent accent fill so the chip stays
    //         legible against busy/dark backgrounds without a hard outline.
    public static string Theme(string theme) =>
        theme switch
        {
            "dark" => "border-accent-500/60 bg-accent-500/10 text-accent-400",
            _ => "border-aesys-800 text-aesys-800",
        };
}
