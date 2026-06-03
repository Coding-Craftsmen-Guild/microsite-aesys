using Microsoft.AspNetCore.Mvc;

namespace Aesys.Core.Components.UI.Button;

public sealed record ButtonViewModel(
    string Label,
    string Href,
    string Target,
    string Variant,
    string Size,
    bool Arrow
);

public sealed class ButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string label,
        string href = "",
        string target = "",
        string variant = "primary",
        string size = "md",
        bool arrow = false
    )
    {
        var vm = new ButtonViewModel(
            Label: label,
            Href: href,
            Target: target,
            Variant: variant,
            Size: size,
            Arrow: arrow
        );

        return View(vm);
    }
}
