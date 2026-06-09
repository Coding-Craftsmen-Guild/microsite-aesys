using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Components.HomePage.Cards;

public sealed record CardItemViewModel(MediaWithCrops Icon, int? Number, string Title, string Text);

public sealed record CardsViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    int PerRow,
    IReadOnlyList<CardItemViewModel> Items
);

public sealed class CardsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Cards source)
    {
        var items =
            source
                .Items?.Select(b => b.Content)
                .OfType<Models.CardItem>()
                // An unset integer reads back as 0; treat that as "no number" so
                // the circle falls through to empty when no icon is set either.
                .Select(c => new CardItemViewModel(
                    c.Icon,
                    c.Number > 0 ? c.Number : null,
                    c.Title,
                    c.Text
                ))
                .ToList()
            ?? [];

        var vm = new CardsViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            PerRow: ParsePerRow(source.PerRow),
            Items: items
        );

        return View(vm);
    }

    private static int ParsePerRow(string value) =>
        int.TryParse(value, out var n) && n is >= 2 and <= 4 ? n : 3;
}
