using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Aesys.Core.Compositions.IntroText;

public sealed record IntroTextViewModel(
    string Eyebrow,
    string Title,
    IHtmlEncodedString Text,
    Link Button,
    string Theme,
    string ClassName
);

public sealed class IntroTextViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        IIntroText source,
        string theme = "light",
        string className = ""
    )
    {
        var vm = new IntroTextViewModel(
            Eyebrow: source.Eyebrow,
            Title: source.Title,
            Text: source.Text,
            Button: source.Button,
            Theme: theme,
            ClassName: className
        );

        return View(vm);
    }
}
