using Aesys.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Aesys.Core.Services;

// One navigation node: a published, non-hidden page plus one level of children.
// Children is empty for leaf nodes and for nodes past the configured scan depth.
// IsActive is true when this node is the current request's page or when any of
// its children is — so a section stays highlighted while you're on a sub-page.
public sealed record NavItem(
    string Label,
    string Url,
    bool IsActive,
    IReadOnlyList<NavItem> Children
);

// Builds the site's top navigation tree from the Umbraco document navigation
// structure. Owns the "scan children, then scan their children one level deeper"
// traversal so the Header ViewComponent stays a thin mapper.
public interface INavigationService
{
    // The HomePage root as the first item, followed by its published, non-hidden
    // children — each child carrying its own published, non-hidden children one
    // level deep (grandchildren). Empty when there is no HomePage root.
    IReadOnlyList<NavItem> GetTopNavigation();
}

public sealed class NavigationService(
    UmbracoHelper umbracoHelper,
    IUmbracoContextAccessor umbracoContextAccessor,
    IDocumentNavigationQueryService navigationQueryService
) : INavigationService
{
    public IReadOnlyList<NavItem> GetTopNavigation()
    {
        var root = umbracoHelper.ContentAtRoot().OfType<Models.HomePage>().FirstOrDefault();
        if (root is null)
        {
            return [];
        }

        var current = CurrentPage();

        var items = new List<NavItem> { new(root.Name, root.Url(), current?.Id == root.Id, []) };
        items.AddRange(BuildChildren(root.Key, current, includeGrandchildren: true));
        return items;
    }

    private IPublishedContent CurrentPage() =>
        umbracoContextAccessor.TryGetUmbracoContext(out var ctx)
            ? ctx.PublishedRequest?.PublishedContent
            : null;

    private List<NavItem> BuildChildren(
        Guid parentKey,
        IPublishedContent current,
        bool includeGrandchildren
    )
    {
        if (!navigationQueryService.TryGetChildrenKeys(parentKey, out var childKeys))
        {
            return [];
        }

        return childKeys
            .Select(umbracoHelper.Content)
            .OfType<Page>()
            .Where(page => page.IsPublished() && !page.HideFromNavigation)
            .Select(page =>
            {
                var children = includeGrandchildren
                    ? BuildChildren(page.Key, current, includeGrandchildren: false)
                    : [];
                var isActive = current?.Id == page.Id || children.Any(c => c.IsActive);
                return new NavItem(page.Name, page.Url(), isActive, children);
            })
            .ToList();
    }
}
