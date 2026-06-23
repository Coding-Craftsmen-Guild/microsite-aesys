using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Shared.Map;

public sealed record MapViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    string MapEmbed,
    string MapPosition
);

public sealed class MapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Map source)
    {
        var vm = new MapViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            MapEmbed: source.MapEmbed,
            MapPosition: source.MapPosition
        );

        return View(vm);
    }
}
