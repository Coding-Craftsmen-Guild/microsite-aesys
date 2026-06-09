using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Shared.TextWithImage;

public sealed record TextWithImageViewModel(
    IIntroText Intro,
    MediaWithCrops Image,
    MediaWithCrops Background,
    string BackgroundColor,
    string ImagePosition
);

public sealed class TextWithImageViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.TextWithImage source)
    {
        var vm = new TextWithImageViewModel(
            Intro: source,
            Image: source.Image,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            ImagePosition: source.ImagePosition
        );

        return View(vm);
    }
}
