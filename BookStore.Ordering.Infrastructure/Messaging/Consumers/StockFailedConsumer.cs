using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Domain.Enums;
using BookStore.Ordering.Infrastructure.Messaging.Connection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace BookStore.Ordering.Infrastructure.Messaging.Consumers;

public class StockFailedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConnection _rabbitMqConnection;

    public StockFailedConsumer(
        IServiceScopeFactory scopeFactory,
        IRabbitMqConnection rabbitMqConnection)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqConnection = rabbitMqConnection;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var connection = _rabbitMqConnection.GetConnection();

        var channel =await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "stock-failed",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var json =
                    Encoding.UTF8.GetString(ea.Body.ToArray());

                var message =
                    JsonSerializer.Deserialize<StockFailedEvent>(json);

                if (message is not null)
                {
                    using var scope =
                        _scopeFactory.CreateScope();

                    var orderRepository =
                        scope.ServiceProvider
                             .GetRequiredService<IOrderRepository>();

                    var unitOfWork =
                        scope.ServiceProvider
                             .GetRequiredService<IUnitOfWork>();

                    var order =
                        await orderRepository
                            .GetByIdAsync(message.OrderId);

                    if (order is not null)
                    {
                        order.Status = OrderStatus.Failed;

                        orderRepository.Update(order);

                        await unitOfWork.SaveChangesAsync(
                            CancellationToken.None);
                    }
                }

                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    false);
            }
            catch
            {
                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    false,
                    true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: "stock-failed",
            autoAck: false,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}