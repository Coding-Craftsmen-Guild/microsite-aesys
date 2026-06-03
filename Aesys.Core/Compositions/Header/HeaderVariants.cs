namespace Aesys.Core.Compositions.Header;

public static class HeaderVariants
{
    public const string Base =
        "flex items-center justify-between gap-4 px-6 py-4 border-b border-aesys-100";

    public const string Logo = "max-h-10 w-auto";

    public const string Toggle =
        "inline-flex items-center justify-center rounded-md p-2 text-aesys-700 hover:bg-aesys-50 focus-visible:ring-2 focus-visible:ring-aesys-500";

    public static string Size(string size) =>
        size switch
        {
            "sm" => "h-12",
            "lg" => "h-20",
            _ => "h-16",
        };
}
