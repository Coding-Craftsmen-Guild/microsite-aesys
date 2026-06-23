using Aesys.Core.Localization;
using Aesys.Core.Notifications;
using Aesys.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Umbraco.Cms.Core.Notifications;

namespace Aesys.Core.Extensions;

public static class CoreRegistrationExtensions
{
    public static IUmbracoBuilder RegisterCore(this IUmbracoBuilder builder)
    {
        builder.AddNotificationAsyncHandler<
            UmbracoApplicationStartedNotification,
            EnsurePageTemplatesHandler
        >();

        builder.Services.AddScoped<IBlogListingService, BlogListingService>();
        builder.Services.AddScoped<IContactEmailService, ContactEmailService>();
        builder.Services.AddScoped<INavigationService, NavigationService>();

        // Localization read layer: one dictionary-backed IStringLocalizer shared by
        // DataAnnotations (via the factory), ILocalizer (C#) and @Html.T (Razor).
        builder.Services.AddSingleton<DictionaryStringLocalizer>();
        builder.Services.AddSingleton<IStringLocalizerFactory, DictionaryStringLocalizerFactory>();
        builder.Services.AddSingleton<IStringLocalizer>(sp =>
            sp.GetRequiredService<DictionaryStringLocalizer>()
        );
        builder.Services.AddSingleton<ILocalizer, Localizer>();

        // Route DataAnnotations text ([Display(Name)], ErrorMessage) through the
        // dictionary-backed factory. AddDataAnnotationsLocalization registers the
        // localization metadata provider that consumes DataAnnotationLocalizerProvider;
        // AddControllersWithViews is idempotent (Umbraco already added MVC, this
        // augments the existing builder rather than duplicating it).
        builder
            .Services.AddControllersWithViews()
            .AddDataAnnotationsLocalization(o =>
                o.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(type)
            );

        return builder;
    }
}
