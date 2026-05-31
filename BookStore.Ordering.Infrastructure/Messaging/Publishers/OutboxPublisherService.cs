using BookStore.Ordering.Application.Interfaces;
using Domain.Entities.IntegrationEventLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using Polly;
using Shared.Contracts.Events;
using System.Text.Json;

namespace BookStore.Ordering.Infrastructure.Messaging.Publishers;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<OutboxPublisherService> _logger;
    public OutboxPublisherService(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i * 2));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var events = await repo.GetPendingEventsAsync(10);

            foreach (var eventLog in events)
            {
                try
                {
                    repo.MarkAsProcessing(eventLog);
                    await uow.SaveChangesAsync(stoppingToken);

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var eventType = Type.GetType(eventLog.EventType)!;

                        var message =
                            JsonSerializer.Deserialize(
                                eventLog.EventData,
                                eventType)!;

                        var queue =
                            GetQueueName(eventType);

                        _logger.LogInformation("Publishing event {EventType}",eventLog.EventType);

                        await bus.PublishAsync(
                            (dynamic)message,
                            queue);

                    repo.MarkAsPublished(eventLog);

                    await uow.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Event {EventType} published successfully", eventLog.EventType);

                    });

                }
                catch (Exception ex)
                {

                    repo.MarkAsFailed(eventLog);

                    await uow.SaveChangesAsync(stoppingToken);

                    _logger.LogError(ex, "Failed to publish event {EventType}", eventLog.EventType);

                }
            }
            await Task.Delay(5000, stoppingToken);
        }
    }

    private static string GetQueueName(Type type) => type.Name switch
    {
        nameof(OrderCreatedEvent) => "order-created",
        nameof(StockReservedEvent) => "stock-reserved",
        nameof(StockFailedEvent) => "stock-failed",
        _ => throw new Exception("Unknown event")
    };
}