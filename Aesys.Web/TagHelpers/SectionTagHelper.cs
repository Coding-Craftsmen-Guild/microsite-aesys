using System.Text.Encodings.Web;
using Aesys.Core.Components.UI.Section;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TailwindMerge;

namespace Aesys.Web.TagHelpers;

// Code-only section wrapper. Renders the standard outer chrome (dark surface,
// optional full-bleed background image + scrim) and an inner `.wrapper` so the
// Razor children land in the centered, padded container. Authored as a tag
// helper (not a ViewComponent) because only tag helpers accept Razor children.
//
//   <section-block bg-image="@Model.Bg.Url()" class="py-20">
//       <h2 class="text-h1">@Model.Title</h2>
//       @await Component.InvokeAsync("IntroText", new { source })
//   </section-block>
[HtmlTargetElement("section-block")]
public sealed class SectionTagHelper(TwMerge twMerge) : TagHelper
{
    [HtmlAttributeName("bg-image")]
    public string BgImage { get; set; }

    // Opt out of the auto header-offset (e.g. when this section is never the
    // first block on a page). Defaults to participating in the offset handshake.
    [HtmlAttributeName("header-offset")]
    public bool HeaderOffset { get; set; } = true;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "section";
        output.TagMode = TagMode.StartTagAndEndTag;

        // Every other attribute on <section-block> (id, aria-*, data-*) passes
        // through to <section> automatically. Only default data-component when
        // the caller didn't set their own (e.g. HeroBanner keeps "hero-banner").
        if (!output.Attributes.ContainsName("data-component"))
        {
            output.Attributes.SetAttribute("data-component", "section");
        }

        var hasImage = !string.IsNullOrWhiteSpace(BgImage);

        // With an image: dark full-bleed chrome. Without: a plain light section
        // with a gray bottom border. `class` written on <section-block> is folded
        // in via TwMerge so per-instance overrides resolve conflicts.
        var chrome = hasImage ? SectionVariants.WithImage : SectionVariants.Plain;
        var extra = output.Attributes["class"]?.Value?.ToString() ?? string.Empty;
        var merged = twMerge.Merge(chrome, OffsetClass(), extra) ?? string.Empty;
        output.Attributes.SetAttribute("class", merged);

        var children = await output.GetChildContentAsync();

        var html = output.Content;
        if (hasImage)
        {
            var url = HtmlEncoder.Default.Encode(BgImage);
            html.AppendHtml($"<img src=\"{url}\" alt=\"\" class=\"{SectionVariants.BgImage}\" />");
            html.AppendHtml($"<div class=\"{SectionVariants.Scrim}\"></div>");
        }

        // The plain (no-image) section's separating hairline goes on the inner
        // wrapper so it aligns to the content gutters, not the full-bleed edge.
        var inner = hasImage
            ? SectionVariants.Inner
            : twMerge.Merge(SectionVariants.Inner, SectionVariants.InnerBorder);
        html.AppendHtml($"<div class=\"{inner}\">");
        html.AppendHtml(children);
        html.AppendHtml("</div>");
    }

    // Mirror HtmlClassExtensions.HeaderOffset: read-and-clear the per-request
    // "first component" flag so exactly one section clears the fixed header.
    private string OffsetClass()
    {
        if (!HeaderOffset)
        {
            return string.Empty;
        }

        var items = ViewContext.HttpContext.Items;
        if (items["IsFirstComponent"] is true)
        {
            items["IsFirstComponent"] = false;
            return "pt-20";
        }

        return string.Empty;
    }
}
