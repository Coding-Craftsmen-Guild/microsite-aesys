using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Shared.Cabinets;

public sealed record CabinetItemViewModel(
    MediaWithCrops Image,
    string Title,
    string Description,
    Link Link
);

public sealed record CabinetsViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    IReadOnlyList<CabinetItemViewModel> Items
);

public sealed class CabinetsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Cabinets source)
    {
        var items =
            source
                .Items?.Select(b => b.Content)
                .OfType<Models.CabinetItem>()
                .Select(c => new CabinetItemViewModel(c.Image, c.Title, c.Description, c.Link))
                .ToList()
            ?? [];

        var vm = new CabinetsViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            Items: items
        );

        return View(vm);
    }
}
