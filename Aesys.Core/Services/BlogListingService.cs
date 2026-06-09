using Aesys.Core.Components.BlogLandingPage.BlogListingCards;
using Aesys.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Aesys.Core.Services;

// Reads the Umbraco context to resolve the blog source for a Blog Listing Cards
// block and pages it into card view models. Owns all the "which page am I on /
// where do the blogs come from" logic so the ViewComponent and the Load-More
// controller stay thin.
//
// The block can sit on the Blog Landing Page itself or on any descendant page;
// either way the blogs come from the nearest Blog Landing Page ancestor (or the
// current page if it *is* a landing page), newest first by the Article Date field.
public interface IBlogListingService
{
    int DefaultPageSize { get; }

    // Initial render: resolve the current request's page from the Umbraco context
    // and return its first page of cards. maxItems is the block's "Max Items"
    // (0/empty falls back to DefaultPageSize).
    BlogCardsViewModel GetForCurrentPage(int maxItems);

    // Load-More: resolve a specific page by id (the listing page the button is
    // anchored to) and return the requested slice.
    BlogCardsViewModel GetForPage(int pageId, int skip, int take);
}

public sealed class BlogListingService(IUmbracoContextAccessor umbracoContextAccessor)
    : IBlogListingService
{
    public int DefaultPageSize => 3;

    public BlogCardsViewModel GetForCurrentPage(int maxItems)
    {
        var current = CurrentPage();
        return Page(current, skip: 0, take: NormalizePageSize(maxItems));
    }

    public BlogCardsViewModel GetForPage(int pageId, int skip, int take)
    {
        var page = umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.Content?.GetById(pageId)
            : null;

        return Page(page, skip, take <= 0 ? DefaultPageSize : take);
    }

    private int NormalizePageSize(int maxItems) => maxItems > 0 ? maxItems : DefaultPageSize;

    // The page rendering the current request.
    private IPublishedContent CurrentPage() =>
        umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.PublishedRequest?.PublishedContent
            : null;

    // Walk from the page up to (and including) itself to find the landing page
    // that owns the blog list. Null when there's no landing page in the ancestry
    // (block dropped somewhere unexpected) — caller renders an empty grid.
    private static Models.BlogLandingPage ResolveLandingPage(IPublishedContent page)
    {
        if (page is null)
        {
            return null;
        }

        return page as Models.BlogLandingPage
            ?? page.Ancestors().OfType<Models.BlogLandingPage>().FirstOrDefault();
    }

    // All published blog children of the landing page, newest first.
    private static IReadOnlyList<Models.BlogPage> OrderedBlogs(Models.BlogLandingPage landing) =>
        landing is null
            ? []
            : landing
                .Children<Models.BlogPage>()
                ?.Where(b => b.IsPublished())
                .OrderByDescending(b => b.ArticleDate)
                .ToList()
                ?? [];

    // Page the ordered blogs. PageId anchors the next Load-More request to the
    // same landing page; NextSkip is the cursor the server-rendered button will
    // carry into its next call.
    private static BlogCardsViewModel Page(IPublishedContent page, int skip, int take)
    {
        var landing = ResolveLandingPage(page);
        var all = OrderedBlogs(landing);

        var items = all.Skip(skip).Take(take).Select(Map).ToList();
        var hasMore = all.Count > skip + take;
        var pageId = landing?.Id ?? page?.Id ?? 0;

        return new BlogCardsViewModel(
            items,
            hasMore,
            pageId,
            NextSkip: skip + take,
            PageSize: take
        );
    }

    private static BlogCardViewModel Map(Models.BlogPage blog) =>
        new(
            ImageUrl: blog.PreviewImage?.Url() ?? string.Empty,
            Date: blog.ArticleDate,
            Title: string.IsNullOrWhiteSpace(blog.Name) ? blog.Title : blog.Name,
            Excerpt: blog.PreviewText ?? string.Empty,
            Url: blog.Url()
        );
}
