using Aesys.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace Aesys.Core.Components.BlogLandingPage.BlogListingCards;

// Load-More endpoint for the Blog Listing Cards block. Routed as a surface
// controller at /umbraco/surface/BlogListing/Cards. Returns the next batch of
// card markup plus a fresh server-rendered Load-More button (the shared _Cards
// partial). The client appends the cards and swaps in the new button — which the
// server omits on the final page — so "no more" needs no out-of-band header.
//
// All page-resolution and paging lives in IBlogListingService; the controller is
// just the HTTP seam.
public sealed class BlogListingController(
    IUmbracoContextAccessor umbracoContextAccessor,
    IUmbracoDatabaseFactory databaseFactory,
    ServiceContext services,
    AppCaches appCaches,
    IProfilingLogger profilingLogger,
    IPublishedUrlProvider publishedUrlProvider,
    IBlogListingService blogListing
)
    : SurfaceController(
        umbracoContextAccessor,
        databaseFactory,
        services,
        appCaches,
        profilingLogger,
        publishedUrlProvider
    )
{
    [HttpGet]
    public IActionResult Cards(int pageId, int skip = 0, int take = 0)
    {
        var result = blogListing.GetForPage(pageId, skip, take);
        return PartialView("Components/BlogListingCards/_Cards", result);
    }
}
