using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Shared.ContactForm;

public sealed record ContactFormViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    string FormTitle,
    string FormSubtitle,
    string Recipients
);

public sealed class ContactFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Models.ContactForm source)
    {
        var vm = new ContactFormViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            FormTitle: source.FormTitle,
            FormSubtitle: source.FormSubtitle,
            Recipients: source.Recipients
        );

        return View(vm);
    }
}
