using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Components.HomePage.Solutions;

public sealed record SolutionItemViewModel(int Index, string Numeral, string Text);

public sealed record SolutionsViewModel(
    IIntroText Intro,
    MediaWithCrops Overlay,
    MediaWithCrops Background,
    string BackgroundColor,
    IReadOnlyList<SolutionItemViewModel> Items
);

public sealed class SolutionsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.Solutions source)
    {
        var items =
            source
                .Items?.Where(text => !string.IsNullOrWhiteSpace(text))
                .Select((text, index) => new SolutionItemViewModel(index, ToRoman(index + 1), text))
                .ToList()
            ?? [];

        var vm = new SolutionsViewModel(
            Intro: source,
            Overlay: source.Overlay,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            Items: items
        );

        return View(vm);
    }

    private static readonly (int Value, string Symbol)[] RomanMap =
    [
        (1000, "M"),
        (900, "CM"),
        (500, "D"),
        (400, "CD"),
        (100, "C"),
        (90, "XC"),
        (50, "L"),
        (40, "XL"),
        (10, "X"),
        (9, "IX"),
        (5, "V"),
        (4, "IV"),
        (1, "I"),
    ];

    private static string ToRoman(int number)
    {
        var builder = new System.Text.StringBuilder();
        foreach (var (value, symbol) in RomanMap)
        {
            while (number >= value)
            {
                builder.Append(symbol);
                number -= value;
            }
        }

        return builder.ToString();
    }
}
