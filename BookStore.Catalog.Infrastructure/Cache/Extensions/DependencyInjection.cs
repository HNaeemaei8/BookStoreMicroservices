using BookStore.Catalog.Application.Interfaces;
using BookStore.Catalog.Infrastructure.Cache;
using BookStore.Catalog.Infrastructure.Messaging.Connection;
using BookStore.Catalog.Infrastructure.Messaging.Consumers;
using BookStore.Catalog.Infrastructure.Persistence;
using BookStore.Catalog.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BookStore.Catalog.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("CatalogDb")));

        services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(
        configuration.GetConnectionString("Redis")!));
       services.AddCaching(configuration);
        services.AddScoped<IBookRepository, BookRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IBookCacheService, BookCacheService>();

        services.AddSingleton<IRabbitMqConnection>(
            _ => new RabbitMqConnection(
                configuration["RabbitMq:Host"]!,
                int.Parse(configuration["RabbitMq:Port"]!),
                configuration["RabbitMq:Username"]!,
                configuration["RabbitMq:Password"]!));

        services.AddScoped<IMessageBus, RabbitMqMessageBus>();
        services.AddHostedService<OrderCreatedConsumer>();
        return services;
    }
}