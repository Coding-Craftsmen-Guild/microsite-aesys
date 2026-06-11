using Aesys.Core.Notifications;
using Aesys.Core.Services;
using Microsoft.Extensions.DependencyInjection;
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

        return builder;
    }
}
