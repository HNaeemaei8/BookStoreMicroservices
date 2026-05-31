
using BookStore.Shared.Common.Results;
using MediatR;

public record CreateOrderCommand(Guid BookId, int Quantity)
    : IRequest<Result<Guid>>;