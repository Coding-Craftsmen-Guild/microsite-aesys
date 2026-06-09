using Aesys.Core.Components.BlogLandingPage.BlogListingCards;
using Aesys.Core.Notifications;
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

        return builder;
    }
}
