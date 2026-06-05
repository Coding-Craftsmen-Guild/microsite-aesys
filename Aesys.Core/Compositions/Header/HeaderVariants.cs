namespace Aesys.Core.Compositions.Header;

public static class HeaderVariants
{
    // Fixed bar that overlays the hero (no flow space) and is transparent at the top of
    // the page so it blends into the navy hero. The `scrolled` class (toggled by header.ts
    // past the scroll threshold) fades in a translucent navy background + backdrop blur —
    // see header.scss. z-40 keeps it above page content, below modals.
    public const string Base = "fixed inset-x-0 top-0 z-40 transition-colors duration-300";

    // White nav link; active = bold accent. Used in both the desktop bar and the
    // mobile drawer, with C# picking the active variant — kept as a helper because
    // it's a genuine repeat plus conditional logic (the rest is inlined in the view).
    public static string NavLink(bool active) =>
        active
            ? "text-base font-bold text-accent-500 transition"
            : "text-base font-normal text-white transition hover:text-accent-500";
}
