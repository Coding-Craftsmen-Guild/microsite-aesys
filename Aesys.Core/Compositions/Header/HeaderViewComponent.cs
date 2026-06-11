using Aesys.Core.Compositions.LanguageSelector;
using Aesys.Core.Models;
using Aesys.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Aesys.Core.Compositions.Header;

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
    INavigationService navigationService
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var root = umbracoHelper.ContentAtRoot().OfType<Models.HomePage>().FirstOrDefault();

        IPublishedContent current = umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.PublishedRequest?.PublishedContent
            : null;

        var currentCulture = variationContextAccessor.VariationContext?.Culture ?? string.Empty;

        var vm = new HeaderViewModel(
            Logo: root?.Logo,
            SiteName: root?.SiteName,
            Items: navigationService.GetTopNavigation(),
            Languages: LanguageOptions.For(current ?? root, currentCulture),
            Button: root?.Button
        );

        return View(vm);
    }
}
