using BookStore.Shared.API.Messaging;
using BookStore.Shared.API.Middleware;
using BookStore.Shared.Common.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookStore.Shared.API.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApi(
      this IServiceCollection services,
      Action<SharedApiOptions>? configure = null)
    {
        if (configure is not null)
            services.Configure(configure);

        services.AddSingleton<IAppMessageProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SharedApiOptions>>().Value;
            return new InMemoryAppMessageProvider(options.Messages);
        });

        return services;
    }


    public static WebApplication UseSharedApi(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
    public sealed class SharedApiOptions
    {
        public IDictionary<string, string> Messages { get; set; } = new Dictionary<string, string>();
    }


}