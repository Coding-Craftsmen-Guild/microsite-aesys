namespace Aesys.Core.Components.UI.Section;

public static class SectionVariants
{
    // With a background image: stacking context for the absolutely-positioned
    // image + scrim, clipped corners, dark navy surface with light text (mirrors
    // HeroBanner). Override colours per-instance via the `class` attribute —
    // it's TwMerge'd onto this.
    public const string WithImage = "relative isolate overflow-hidden bg-aesys-800 text-white";

    // Without a background image: a plain light section. No forced surface/text
    // colour (inherits the page). The separating hairline lives on the inner
    // wrapper (see InnerBorder) so it aligns to the content gutters, not the
    // full-bleed section edge.
    public const string Plain = "relative isolate";

    // Inner centered container + vertical rhythm. `wrapper` supplies max-width
    // and responsive gutters; relative keeps children above the -z scrim.
    public const string Inner = "wrapper relative py-10 md:py-16";

    // Gray bottom border for plain (no-image) sections, applied to the inner
    // wrapper so it spans the content width rather than the full viewport bleed.
    public const string InnerBorder = "border-b border-aesys-100";

    // Full-bleed background image, pinned behind content (-z-20), with a scrim
    // overlay (-z-10) for text legibility.
    public const string BgImage = "absolute inset-0 -z-20 h-full w-full object-cover";
    public const string Scrim = "absolute inset-0 -z-10 bg-scrim";

    // Surface colour for an image-less section, driven by the Section's
    // Background Color dropdown (None/Navy/Light). Navy is the dark brand
    // surface with light text; Light is the off-white surface; None inherits
    // the page (transparent). TwMerge'd onto Plain, so the colour wins over the
    // base while the positioning/isolation classes survive.
    public static string Surface(string bgColor) =>
        bgColor switch
        {
            "Navy" => "bg-aesys-800 text-white",
            "Light" => "bg-surface-light",
            _ => string.Empty,
        };

    // Background Color implies the text theme passed to IntroText et al.:
    // an image band or a Navy surface reads as "dark"; everything else "light".
    public static string Theme(string bgColor, bool hasImage) =>
        hasImage || bgColor == "Navy" ? "dark" : "light";

    // The separating hairline only makes sense on a plain transparent section.
    // Once the section carries a surface colour (Navy/Light) the colour change
    // is the separator, so suppress the border.
    public static bool HasInnerBorder(string bgColor, bool hasImage) =>
        !hasImage && string.IsNullOrEmpty(Surface(bgColor));
}
