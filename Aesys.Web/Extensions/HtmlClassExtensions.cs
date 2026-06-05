using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using TailwindMerge;

namespace Aesys.Web.Extensions;

public static class HtmlClassExtensions
{
    public static IHtmlContent Cn(this IHtmlHelper html, params string[] classNames)
    {
        var twMerge = html.ViewContext.HttpContext.RequestServices.GetRequiredService<TwMerge>();
        var merged = twMerge.Merge(classNames) ?? string.Empty;
        return new HtmlString(merged);
    }

    // Top padding that clears the fixed, h-20 header — emitted only on the page's first
    // block. The page sets HttpContext.Items["IsFirstComponent"]; this reads-and-clears it
    // so exactly one component (whichever renders first) gets the offset. Returns a class
    // string so the caller can fold it into @Html.Cn(...). Keep pt-20 in sync with the
    // header's h-20 height.
    public static string HeaderOffset(this IHtmlHelper html)
    {
        var items = html.ViewContext.HttpContext.Items;
        if (items["IsFirstComponent"] is true)
        {
            items["IsFirstComponent"] = false;
            return "pt-20";
        }

        return string.Empty;
    }
}
