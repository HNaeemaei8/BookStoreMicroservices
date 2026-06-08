using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Domain.Enums;
using BookStore.Ordering.Infrastructure.Messaging.Connection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace BookStore.Ordering.Infrastructure.Messaging.Consumerers;

public class StockResultConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StockResultConsumer> _logger;

    public StockResultConsumer(
        IRabbitMqConnection connection,
        IServiceScopeFactory scopeFactory,
        ILogger<StockResultConsumer> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var conn = _connection.GetConnection();
        var channel = await conn.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            "stock-result",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<StockResultEvent>(json);

                if (message == null)
                {
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var order = await repo.GetByIdAsync(message.OrderId);

                if (order != null)
                {
                    order.Status = message.IsSuccess ? OrderStatus.Confirmed : OrderStatus.Failed;
                    repo.Update(order);
                    await uow.SaveChangesAsync(CancellationToken.None);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderCreated");

                var retryCount = 0;

                if (ea.BasicProperties?.Headers != null &&
                    ea.BasicProperties.Headers.TryGetValue("retry-count", out var value))
                {
                    retryCount = Convert.ToInt32(value);
                }

                retryCount++;

                if (retryCount > 3)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, false, false); 
                    return;
                }

                var newProps = new BasicProperties
                {
                    Persistent = true,
                    Headers = new Dictionary<string, object>
                    {
                        ["retry-count"] = retryCount
                    }
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "order-created",
                    mandatory: false,
                    basicProperties: newProps,
                    body: ea.Body
                );

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };
        var consumerTag = $"{nameof(StockResultConsumer)}-{Guid.NewGuid()}";

        await channel.BasicConsumeAsync(
            queue: "stock-result",
            autoAck: false,
            consumerTag: consumerTag,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}