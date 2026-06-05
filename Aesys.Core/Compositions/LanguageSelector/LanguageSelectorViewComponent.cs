using Microsoft.AspNetCore.Mvc;

namespace Aesys.Core.Compositions.LanguageSelector;

public sealed record LanguageOption(
    string IsoCode,
    string Short,
    string Name,
    string Url,
    bool IsCurrent
);

public sealed record LanguageSelectorViewModel(IReadOnlyList<LanguageOption> Languages);

public sealed class LanguageSelectorViewComponent : ViewComponent
{
    // The bordered current-language toggle (a `lang`-variant Button), shared by the
    // header and footer.
    public IViewComponentResult Invoke(IReadOnlyList<LanguageOption> languages)
    {
        var vm = new LanguageSelectorViewModel(Languages: languages ?? []);

        return View(vm);
    }
}
