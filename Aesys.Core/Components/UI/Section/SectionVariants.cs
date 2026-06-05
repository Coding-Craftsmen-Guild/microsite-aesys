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
}
