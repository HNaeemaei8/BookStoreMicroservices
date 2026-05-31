using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Command;

public sealed record UpdateBookCommand(
    Guid Id,
    string Title,
    string Author,
    decimal Price,
    int Stock
) : IRequest<Result>;