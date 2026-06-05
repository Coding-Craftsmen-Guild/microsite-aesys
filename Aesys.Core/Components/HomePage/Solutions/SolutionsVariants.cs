namespace Aesys.Core.Components.HomePage.Solutions;

public static class SolutionsVariants
{
    // The numbered chip alternates by item position: even indexes (1st, 3rd, ...)
    // are navy-filled with a light numeral; odd indexes (2nd, 4th, ...) are a 2px
    // green outline on a transparent surface with a green numeral. `rounded-lg`
    // and `text-chip` are the design-system "numbered chip" tokens.
    public const string Chip =
        "flex size-16 shrink-0 items-center justify-center rounded-lg text-chip font-extrabold leading-none";

    public static string ChipFill(int index) =>
        index % 2 == 0
            ? "bg-aesys-800 text-white"
            : "border-2 border-accent-500 bg-transparent text-accent-500";
}
