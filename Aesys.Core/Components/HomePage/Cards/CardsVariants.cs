namespace Aesys.Core.Components.HomePage.Cards;

public static class CardsVariants
{
    // The card surface. Rounded with generous padding; pt is larger so the
    // top-left circle (which overlaps the card edge) clears the title. The
    // circle is absolutely positioned against this, so the card is `relative`.
    public const string Card = "relative rounded-xl p-8 pt-12";

    // Card surface + text colour by section theme. On a dark section the card
    // is navy with white text; on a light section it's a pale surface with the
    // brand navy text and a hairline border to lift it off the page.
    public static string CardFill(string theme) =>
        theme == "dark"
            ? "bg-aesys-800 text-white"
            : "bg-surface-light text-aesys-800 border border-aesys-100";

    // The circle holding the icon/number, pinned to the top-left and pulled up
    // and left so it straddles the card's edge (the "badge" look).
    public const string Circle =
        "absolute -left-4 -top-6 flex size-16 shrink-0 items-center justify-center rounded-full text-chip font-extrabold leading-none";

    // Circle surface by theme. On a dark section the circle is white with navy
    // contents; on a light section it's navy with white contents.
    public static string CircleFill(string theme) =>
        theme == "dark" ? "bg-white text-aesys-800" : "bg-aesys-800 text-white";

    // Desktop columns. Mobile always stacks to one column. Literal classes so
    // Tailwind's source scan picks every variant up.
    public static string Columns(int perRow) =>
        perRow switch
        {
            2 => "grid grid-cols-1 gap-8 md:grid-cols-2",
            4 => "grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-4",
            _ => "grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-3",
        };
}
