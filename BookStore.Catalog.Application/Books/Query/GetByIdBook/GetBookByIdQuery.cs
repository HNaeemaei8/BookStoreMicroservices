using BookStore.Catalog.Application.Books.Dtos;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Query
{
    public sealed record GetBookByIdQuery(Guid Id)
        : IRequest<Result<BookDto>>;
}
