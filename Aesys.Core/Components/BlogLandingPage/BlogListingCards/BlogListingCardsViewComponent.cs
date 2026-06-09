using Aesys.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Aesys.Core.Components.BlogLandingPage.BlogListingCards;

public sealed record BlogListingCardsViewModel(
    IIntroText Intro,
    MediaWithCrops Background,
    string BackgroundColor,
    BlogCardsViewModel Cards
);

public sealed class BlogListingCardsViewComponent(IBlogListingService blogListing) : ViewComponent
{
    public IViewComponentResult Invoke(Models.BlogListingCards source)
    {
        var vm = new BlogListingCardsViewModel(
            Intro: source,
            Background: source.Background,
            BackgroundColor: source.BackgroundColor,
            Cards: blogListing.GetForCurrentPage(source.MaxItems)
        );

        return View(vm);
    }
}
