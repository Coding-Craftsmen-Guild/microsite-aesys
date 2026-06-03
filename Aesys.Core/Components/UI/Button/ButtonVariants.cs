namespace Aesys.Core.Components.UI.Button;

public static class ButtonVariants
{
    // Pill geometry shared by every button: full-radius, centered, 14px label,
    // transition + accent focus ring. Height/padding come from Size().
    public const string Base =
        "inline-flex items-center justify-center gap-2 rounded-full text-sm font-bold leading-none whitespace-nowrap transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-accent-500 focus-visible:ring-offset-2";

    // Variants map to the design system's button taxonomy (docs/design-system.md):
    //   primary  — solid accent-green pill, white label (the band/header CTAs)
    //   outline  — white pill, 1px accent border, navy label (+ green arrow)
    //   lang     — transparent pill, 1px white border, white label (the "SRB" toggle)
    //   tag      — transparent pill, 1px accent border, accent label (blog tags)
    public static string Variant(string variant) =>
        variant switch
        {
            "outline" => "bg-white border border-accent-500 text-aesys-800 hover:bg-accent-50",
            "lang" => "bg-transparent border border-white text-white hover:bg-white/10",
            "tag" =>
                "bg-transparent border border-accent-500 text-accent-500 hover:bg-accent-500/10",
            _ => "bg-accent-500 text-white border border-transparent hover:bg-accent-600",
        };

    // Fixed pill heights from the design: 41px default (h-control), 30px small
    // (h-control-sm — the lang toggle / tag pills). Width is intrinsic via px.
    public static string Size(string size) =>
        size switch
        {
            "sm" => "h-control-sm px-4",
            _ => "h-control px-6",
        };

    // The accent green ➤ that trails most CTA labels. On the solid `primary`
    // variant the whole label (arrow included) is white via currentColor.
    public static string ArrowClass(string variant) =>
        variant == "primary" ? "" : "text-accent-500";
}
