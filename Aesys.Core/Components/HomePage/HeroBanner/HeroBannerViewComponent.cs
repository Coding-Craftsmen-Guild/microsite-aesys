using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Components.HomePage.HeroBanner;

public sealed record HeroFeatureViewModel(MediaWithCrops Icon, string Text);

public sealed record HeroStatViewModel(string Value, string Label);

public sealed record HeroBannerViewModel(
    string Eyebrow,
    string Title,
    string Text,
    Link Button,
    IReadOnlyList<HeroFeatureViewModel> Features,
    IReadOnlyList<HeroStatViewModel> Stats,
    MediaWithCrops Background,
    string Footnote
);

public sealed class HeroBannerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.HeroBanner source)
    {
        var features =
            source
                .Features?.Select(b => b.Content)
                .OfType<Models.HeroFeature>()
                .Select(f => new HeroFeatureViewModel(f.Icon, f.Text))
                .ToList()
            ?? [];

        var stats =
            source
                .Stats?.Select(b => b.Content)
                .OfType<Models.HeroStat>()
                .Select(s => new HeroStatViewModel(s.Value, s.Label))
                .ToList()
            ?? [];

        var vm = new HeroBannerViewModel(
            Eyebrow: source.Eyebrow,
            Title: source.Title,
            Text: source.Text,
            Button: source.Button,
            Features: features,
            Stats: stats,
            Background: source.Background,
            Footnote: source.Footnote
        );

        return View(vm);
    }
}
