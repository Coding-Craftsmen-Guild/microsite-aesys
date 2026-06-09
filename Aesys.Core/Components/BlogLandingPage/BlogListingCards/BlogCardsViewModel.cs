namespace Aesys.Core.Components.BlogLandingPage.BlogListingCards;

// One blog card's data, mapped off a BlogPage. Rendered by the _Cards partial
// (shared by the initial ViewComponent render and the Load-More controller).
public sealed record BlogCardViewModel(
    string ImageUrl,
    DateTime Date,
    string Title,
    string Excerpt,
    string Url
);

// A page (batch) of blog cards plus the paging context the _Cards partial needs
// to render the server-authoritative Load-More button. Both the initial
// ViewComponent render and the Load-More controller return this same partial, so
// each response carries its own fresh button (advanced NextSkip) — or no button
// at all when !HasMore. The JS never computes the cursor; it just swaps in
// whatever the server sent.
public sealed record BlogCardsViewModel(
    IReadOnlyList<BlogCardViewModel> Items,
    bool HasMore,
    int PageId,
    int NextSkip,
    int PageSize
);
