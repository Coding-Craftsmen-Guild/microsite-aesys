using Aesys.Core.Compositions.LanguageSelector;
using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Aesys.Core.Compositions.Header;

public sealed record NavItem(string Label, string Url, bool IsActive);

public sealed record HeaderViewModel(
    MediaWithCrops Logo,
    string SiteName,
    IReadOnlyList<NavItem> Items,
    IReadOnlyList<LanguageOption> Languages,
    Link Button
);

public sealed class HeaderViewComponent(
    UmbracoHelper umbracoHelper,
    IUmbracoContextAccessor umbracoContextAccessor,
    IVariationContextAccessor variationContextAccessor,
    IDocumentNavigationQueryService navigationQueryService
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var root = umbracoHelper.ContentAtRoot().OfType<Models.HomePage>().FirstOrDefault();

        IPublishedContent current = umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.PublishedRequest?.PublishedContent
            : null;

        var currentCulture = variationContextAccessor.VariationContext?.Culture ?? string.Empty;

        var items = new List<NavItem>();
        if (root is not null)
        {
            items.Add(new NavItem(root.Name, root.Url(), current?.Id == root.Id));

            if (navigationQueryService.TryGetChildrenKeys(root.Key, out var childKeys))
            {
                items.AddRange(
                    childKeys
                        .Select(umbracoHelper.Content)
                        .OfType<Page>()
                        .Where(page => page.IsPublished() && !page.HideFromNavigation)
                        .Select(page => new NavItem(page.Name, page.Url(), current?.Id == page.Id))
                );
            }
        }

        var vm = new HeaderViewModel(
            Logo: root?.Logo,
            SiteName: root?.SiteName,
            Items: items,
            Languages: LanguageOptions.For(current ?? root, currentCulture),
            Button: root?.Button
        );

        return View(vm);
    }
}
