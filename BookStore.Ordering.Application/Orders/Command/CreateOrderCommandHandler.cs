using BookStore.Ordering.Application.Errors;
using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Domain.Enums;
using BookStore.Shared.Common.Results;
using Domain.Entities.IntegrationEventLog;
using MediatR;
using Shared.Contracts.Events;
using System.Text.Json;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _repo;
    private readonly IOutboxRepository _outbox;
    private readonly IUnitOfWork _uow;

    public CreateOrderCommandHandler(
        IOrderRepository repo,
        IOutboxRepository outbox,
        IUnitOfWork uow)
    {
        _repo = repo;
        _outbox = outbox;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        if (request.Quantity <= 0)
            return Result.Failure<Guid>(Error.Validation("Invalid quantity"));

        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            Quantity = request.Quantity,
            UnitPrice = 0
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem> { item }
        };

        var orderEvent = new OrderCreatedEvent(order.Id, request.BookId, request.Quantity) { CorrelationId = new Guid() };
      
        var log = new IntegrationEventLog
        {
            Id = Guid.NewGuid(),
            EventType = nameof(OrderCreatedEvent),
            EventData = JsonSerializer.Serialize(orderEvent),
            CreatedAt = DateTime.UtcNow,
            Status = EventStatus.Pending,
            CorrelationId = orderEvent.CorrelationId
        };

        await _repo.AddAsync(order);
        _outbox.Add(log);

        await _uow.SaveChangesAsync(ct);

        return Result.Success(order.Id);
    }
}