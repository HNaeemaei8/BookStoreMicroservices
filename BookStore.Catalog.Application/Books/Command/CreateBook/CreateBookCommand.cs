using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Command;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    decimal Price,
    int Stock
) : IRequest<Result<Guid>>;