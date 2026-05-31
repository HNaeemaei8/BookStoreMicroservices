using BookStore.Modules.Ordering.API;
using BookStore.Shared.API.DependencyInjection;

namespace BookStore.Ordering.API.Extention;
public static class OrderingServiceCollectionExtensions
{
    public static IServiceCollection AddOrderingApi(this IServiceCollection services)
    {
        services.AddSharedApi();

        services.Configure<SharedApiOptions>(opt =>
        {
            foreach (var kv in OrderingMessages.All)
                opt.Messages[kv.Key] = kv.Value;
        });

        return services;
    }
}