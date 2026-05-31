using BookStore.Catalog.Application.Interfaces;
using BookStore.Catalog.Infrastructure.Messaging.Connection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace BookStore.Catalog.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    public OrderCreatedConsumer(
        IServiceScopeFactory scopeFactory,
        IRabbitMqConnection rabbitMqConnection, ILogger<OrderCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqConnection = rabbitMqConnection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var connection =
            _rabbitMqConnection.GetConnection();

        var channel =
            await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var json =
                    Encoding.UTF8.GetString(
                        ea.Body.ToArray());

                var message =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                if (message is not null)
                {
                    await ProcessOrder(message);
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
            queue: "order-created",
            autoAck: false,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task ProcessOrder(
        OrderCreatedEvent message)
    {
        _logger.LogInformation("OrderCreated received OrderId={OrderId}",message.OrderId);

        using var scope =
            _scopeFactory.CreateScope();

        var repo =
            scope.ServiceProvider
                 .GetRequiredService<IBookRepository>();

        var bus =
            scope.ServiceProvider
                 .GetRequiredService<IMessageBus>();

        var unitOfWork =
            scope.ServiceProvider
                 .GetRequiredService<IUnitOfWork>();

        var book =
            await repo.GetByIdAsync(message.BookId);

        if (book is null)
        {

            await bus.PublishAsync(
                new StockFailedEvent(
                    message.OrderId,
                    "Book not found",
                    message.CorrelationId),
                "stock-failed");

            _logger.LogWarning("Book not found BookId={BookId}", message.BookId);

            return;
        }

        if (book.Stock < message.Quantity)
        {

            await bus.PublishAsync(
                new StockFailedEvent(
                    message.OrderId,
                    "Not enough stock",
                    message.CorrelationId),
                "stock-failed");

            _logger.LogWarning("Book not found BookId={BookId}", message.BookId);

            return;
        }

        book.Stock -= message.Quantity;

        repo.Update(book);

        await unitOfWork.SaveChangesAsync(CancellationToken.None);


        await bus.PublishAsync(
            new StockReservedEvent(
                message.OrderId,
                message.CorrelationId),
            "stock-reserved");

        _logger.LogInformation("Stock reserved OrderId={OrderId}", message.OrderId);

    }
}