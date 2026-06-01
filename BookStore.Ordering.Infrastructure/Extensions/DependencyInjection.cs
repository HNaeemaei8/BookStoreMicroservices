
using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Infrastructure.Messaging.Connection;
using BookStore.Ordering.Infrastructure.Messaging.Consumers;
using BookStore.Ordering.Infrastructure.Messaging.Publishers;
using BookStore.Ordering.Infrastructure.Persistence;
using BookStore.Ordering.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using RabbitMQ.Client;


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

        services.AddSingleton<IConnectionFactory>(_ =>
            new ConnectionFactory
            {
                HostName = configuration["RabbitMq:Host"],
                Port = int.Parse(configuration["RabbitMq:Port"]!),
                UserName = configuration["RabbitMq:Username"],
                Password = configuration["RabbitMq:Password"],

                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            });

        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

        services.AddHostedService<OutboxPublisherService>();

        services.AddHostedService<StockReservedConsumer>();

        services.AddHostedService<StockFailedConsumer>();

        return services;
    }
}