using BookStore.Ordering.Application.Interfaces;
using Domain.Entities.IntegrationEventLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Shared.Contracts.Events;
using System.Text.Json;

namespace BookStore.Ordering.Infrastructure.Messaging.Publishers;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly IAsyncPolicy _retryPolicy;

    public OutboxPublisherService(IServiceScopeFactory scopeFactory,ILogger<OutboxPublisherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                i => TimeSpan.FromSeconds(i * 2),
                (ex, time, retryCount, ctx) =>
                {
                    _logger.LogWarning(
                        ex,
                        "Retry {RetryCount} for Outbox publish",
                        retryCount);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var events = await repo.GetPendingEventsAsync(10);

                foreach (var eventLog in events)
                {
                    await ProcessEvent(eventLog, repo, bus, uow, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox publisher loop failed");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ProcessEvent(IntegrationEventLog eventLog,IOutboxRepository repo,IMessageBus bus,IUnitOfWork uow,CancellationToken stoppingToken)
    {
        try
        {
            repo.MarkAsProcessing(eventLog);
            await uow.SaveChangesAsync(stoppingToken);

            await _retryPolicy.ExecuteAsync(async () =>
            {
                var eventType = Type.GetType(eventLog.EventType);

                if (eventType == null)
                    throw new Exception($"Unknown event type: {eventLog.EventType}");

                var message = JsonSerializer.Deserialize(eventLog.EventData, eventType);

                if (message == null)
                    throw new Exception($"Invalid event data: {eventLog.EventType}");

                var queue = GetQueueName(eventType);

                _logger.LogInformation(
                    "Publishing event {EventType} to {Queue}",
                    eventLog.EventType,
                    queue);

                await bus.PublishAsync((dynamic)message, queue);
            });

            repo.MarkAsPublished(eventLog);
            await uow.SaveChangesAsync(stoppingToken);

            _logger.LogInformation(
                "Event published successfully {EventType}",
                eventLog.EventType);
        }
        catch (Exception ex)
        {
            repo.MarkAsFailed(eventLog);
            await uow.SaveChangesAsync(stoppingToken);

            _logger.LogError(
                ex,
                "Failed to publish event {EventType}",
                eventLog.EventType);
        }
    }

    private static string GetQueueName(Type type) => type.Name switch
    {
        nameof(OrderCreatedEvent) => "order-created",
        nameof(StockReservedEvent) => "stock-reserved",
        _ => throw new Exception($"Unknown event mapping for {type.Name}")
    };
}