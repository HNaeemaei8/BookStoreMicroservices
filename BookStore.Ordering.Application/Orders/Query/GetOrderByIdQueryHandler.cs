using BookStore.Ordering.Application.Errors;
using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Application.Orders.Dtos;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Ordering.Application.Orders.GetOrderById;

public class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id);

        if (order is null)
            return Result.Failure<OrderDto>(
                OrderErrors.NotFound(request.Id));

        if (order.Items == null || !order.Items.Any())
            return Result.Failure<OrderDto>(
                OrderErrors.NotFound(request.Id));

        var orderDto = new OrderDto
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            OrderDate = order.CreatedAt,

            TotalPrice = order.TotalPrice,

            Items = order.Items.Select(i => new OrderItemDto
            {
                BookId = i.BookId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };

        return Result.Success(orderDto);
    }
}