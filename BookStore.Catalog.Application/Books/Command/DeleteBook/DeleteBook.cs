using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Command;

public sealed record DeleteBookCommand(Guid Id)
    : IRequest<Result>;