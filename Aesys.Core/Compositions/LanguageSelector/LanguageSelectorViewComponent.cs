using Microsoft.AspNetCore.Mvc;

namespace Aesys.Core.Compositions.LanguageSelector;

public sealed record LanguageOption(
    string IsoCode,
    string Short,
    string Name,
    string Url,
    bool IsCurrent
);

public sealed record LanguageSelectorViewModel(
    IReadOnlyList<LanguageOption> Languages,
    string Placement,
    string Align
);

public sealed class LanguageSelectorViewComponent : ViewComponent
{
    // The bordered current-language dropdown (a `lang`-variant pill), shared by the
    // header and footer. `placement` controls which way the menu opens: "down"
    // (default, header) or "up" (footer, which sits at the bottom of the page).
    // `align` is which edge the panel anchors to: "end" (default, right-0) when the
    // pill is at the right (header cluster, footer), or "start" (left-0) when it's
    // at the left (mobile menu) so the panel doesn't clip off the screen edge.
    public IViewComponentResult Invoke(
        IReadOnlyList<LanguageOption> languages,
        string placement = "down",
        string align = "end"
    )
    {
        var vm = new LanguageSelectorViewModel(
            Languages: languages ?? [],
            Placement: placement,
            Align: align
        );

        return View(vm);
    }
}
