
using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Infrastructure.Messaging.Connection;

using BookStore.Ordering.Infrastructure.Persistence;
using BookStore.Ordering.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace BookStore.Ordering.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("OrderingDb")));

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IRabbitMqConnection>(sp =>
        {
            var config =
                configuration.GetSection("RabbitMq");

            var logger =
                sp.GetRequiredService<
                    ILogger<RabbitMqConnection>>();

            return new RabbitMqConnection(
                config["HostName"]!,
                5672,
                config["UserName"]!,
                config["Password"]!,
                logger);
        });

        //services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

        //services.AddHostedService<OutboxPublisherService>();

        //services.AddHostedService<StockReservedConsumer>();

        //services.AddHostedService<StockFailedConsumer>();

        return services;
    }
}