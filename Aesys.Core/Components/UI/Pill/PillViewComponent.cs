using Microsoft.AspNetCore.Mvc;

namespace Aesys.Core.Components.UI.Pill;

public sealed record PillViewModel(string Label, string Theme);

public sealed class PillViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string label, string theme = "light")
    {
        return View(new PillViewModel(Label: label, Theme: theme));
    }
}
