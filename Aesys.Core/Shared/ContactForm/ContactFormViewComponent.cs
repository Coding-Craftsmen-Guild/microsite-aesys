using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;

namespace Aesys.Core.Shared.ContactForm;

public sealed record ContactFormViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    string FormTitle,
    string FormSubtitle,
    // The page this block sits on. The POST carries this instead of the recipient
    // address, so the destination is resolved from content server-side.
    Guid PageKey
);

public sealed class ContactFormViewComponent(IUmbracoContextAccessor umbracoContextAccessor)
    : ViewComponent
{
    public IViewComponentResult Invoke(Models.ContactForm source)
    {
        var current = umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.PublishedRequest?.PublishedContent
            : null;

        var vm = new ContactFormViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            FormTitle: source.FormTitle,
            FormSubtitle: source.FormSubtitle,
            PageKey: current?.Key ?? Guid.Empty
        );

        return View(vm);
    }
}
