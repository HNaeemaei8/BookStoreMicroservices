using BookStore.Catalog.Application.Books.Dtos;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Query
{
    public sealed record GetAllBooksQuery
        : IRequest<Result<List<BookDto>>>;
}
