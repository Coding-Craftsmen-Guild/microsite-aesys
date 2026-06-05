using Aesys.Core.Compositions.LanguageSelector;
using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Aesys.Core.Compositions.Footer;

public sealed record FooterLink(string Label, string Url, string Target, bool Indented);

public sealed record FooterColumnViewModel(string Title, IReadOnlyList<FooterLink> Links);

public sealed record FooterViewModel(
    MediaWithCrops Logo,
    string SiteName,
    string Tagline,
    string Address,
    IReadOnlyList<FooterColumnViewModel> Columns,
    IReadOnlyList<LanguageOption> Languages
);

public sealed class FooterViewComponent(
    UmbracoHelper umbracoHelper,
    IUmbracoContextAccessor umbracoContextAccessor,
    IVariationContextAccessor variationContextAccessor
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var root = umbracoHelper.ContentAtRoot().OfType<Models.HomePage>().FirstOrDefault();

        IPublishedContent current = umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.PublishedRequest?.PublishedContent
            : null;

        var currentCulture = variationContextAccessor.VariationContext?.Culture ?? string.Empty;

        var columns =
            root?.Columns?.Select(b => b.Content)
                .OfType<Models.FooterColumn>()
                .Select(c => new FooterColumnViewModel(
                    Title: c.Title,
                    Links:
                    [
                        .. (c.Links ?? []).Select(l => new FooterLink(
                            Label: l.Name,
                            Url: l.Url,
                            Target: l.Target,
                            Indented: (l.Name ?? string.Empty).TrimStart().StartsWith('—')
                        )),
                    ]
                ))
                .ToList()
            ?? [];

        var vm = new FooterViewModel(
            Logo: root?.Logo,
            SiteName: root?.SiteName,
            Tagline: root?.Tagline,
            Address: root?.Address,
            Columns: columns,
            Languages: LanguageOptions.For(current ?? root, currentCulture)
        );

        return View(vm);
    }
}
