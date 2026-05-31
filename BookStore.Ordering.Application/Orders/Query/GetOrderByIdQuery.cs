using BookStore.Ordering.Application.Orders.Dtos;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Ordering.Application.Orders.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
