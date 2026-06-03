using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common;

namespace Aesys.Core.Compositions.Header;

public sealed record HeaderViewModel(MediaWithCrops Logo);

public sealed class HeaderViewComponent(UmbracoHelper umbracoHelper) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var root = umbracoHelper.ContentAtRoot().OfType<IHeader>().FirstOrDefault();

        var vm = new HeaderViewModel(Logo: root.HeaderLogo);
        
        return View(vm);
    }
}
