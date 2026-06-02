using BookStore.Ordering.Application.Errors;
using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Domain.Enums;
using BookStore.Shared.Common.Results;
using Domain.Entities.IntegrationEventLog;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using System.Text.Json;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _repo;
    private readonly IOutboxRepository _outbox;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    public CreateOrderCommandHandler(
        IOrderRepository repo,
        IOutboxRepository outbox,
        IUnitOfWork uow,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _repo = repo;
        _outbox = outbox;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        if (request.Quantity <= 0)
            return Result.Failure<Guid>(Error.Validation("Invalid quantity"));

        var orderId = Guid.NewGuid();

        var order = new Order
        {
            Id = orderId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                Id = Guid.NewGuid(),
                BookId = request.BookId,
                Quantity = request.Quantity,
                UnitPrice = 0
            }
        }
        };

        var orderEvent = new OrderCreatedEvent
        {
            BookId = request.BookId,
            Quantity = request.Quantity,
            CorrelationId = Guid.NewGuid(),
            OrderId = orderId
        };

        var log = new IntegrationEventLog
        {
            Id = Guid.NewGuid(),
            EventType = nameof(OrderCreatedEvent),
            EventData = JsonSerializer.Serialize(orderEvent),
            CreatedAt = DateTime.UtcNow,
            Status = EventStatus.Pending,
            CorrelationId = orderEvent.CorrelationId
        };

        _logger.LogInformation("Creating order {OrderId}", orderId);

        await _repo.AddAsync(order);
        _outbox.Add(log);

        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Order saved successfully {OrderId}", orderId);

        return Result.Success(orderId);
    }
}