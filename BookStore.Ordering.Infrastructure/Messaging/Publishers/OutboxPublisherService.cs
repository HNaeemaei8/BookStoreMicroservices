using BookStore.Ordering.Application.Interfaces;
using Domain.Entities.IntegrationEventLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Application.Interfaces;
using Polly;
using Shared.Contracts.Events;
using System.Text.Json;

namespace BookStore.Ordering.Infrastructure.Messaging.Publishers;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAsyncPolicy _retryPolicy;

    public OutboxPublisherService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

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
                    await uow.SaveChangesAsync(CancellationToken.None);

                    var eventType = Type.GetType(eventLog.EventType);
                    var message = JsonSerializer.Deserialize(eventLog.EventData, eventType);

                   
                    var queue = GetQueueName(eventType);
                    await bus.PublishAsync((dynamic)message, queue);

                    repo.MarkAsPublished(eventLog);
                    await uow.SaveChangesAsync(CancellationToken.None);
                }
                catch
                {
                    repo.MarkAsFailed(eventLog);
                    await uow.SaveChangesAsync(CancellationToken.None);
                }
            }
            foreach (var e in events)
            {
                try
                {
                    e.Status = EventStatus.Processing;
                    await uow.SaveChangesAsync(stoppingToken);

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var type = Type.GetType(e.EventType)!;

                        var message = JsonSerializer.Deserialize(e.EventData, type)!;

                        var queue = GetQueueName(type);

                        await bus.PublishAsync((dynamic)message, queue);

                        e.Status = EventStatus.Published;
                        await uow.SaveChangesAsync(stoppingToken);
                    });
                }
                catch
                {
                    e.Status = EventStatus.Failed;
                    await uow.SaveChangesAsync(stoppingToken);
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