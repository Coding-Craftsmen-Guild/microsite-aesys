using System.Globalization;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Aesys.Core.Compositions.LanguageSelector;

// Builds the per-culture LanguageOption list for a node, shared by the Header and Footer
// (both surface the same language switcher via the LanguageSelector view component).
public static class LanguageOptions
{
    public static IReadOnlyList<LanguageOption> For(IPublishedContent node, string currentCulture)
    {
        if (node is null)
        {
            return [];
        }

        return
        [
            .. node.Cultures.Values.Select(c => new LanguageOption(
                IsoCode: c.Culture,
                Short: ShortCode(c.Culture),
                Name: DisplayName(c.Culture),
                Url: node.Url(culture: c.Culture),
                IsCurrent: string.Equals(
                    c.Culture,
                    currentCulture,
                    StringComparison.OrdinalIgnoreCase
                )
            )),
        ];
    }

    private static string ShortCode(string iso) =>
        string.IsNullOrEmpty(iso) ? string.Empty : iso.Split('-')[0].ToUpperInvariant();

    private static string DisplayName(string iso)
    {
        try
        {
            var name = CultureInfo.GetCultureInfo(iso).NativeName;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }
        catch (CultureNotFoundException)
        {
            return iso;
        }
    }
}
