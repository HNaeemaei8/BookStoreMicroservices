using BookStore.Catalog.Application.Books.Dtos;
using BookStore.Catalog.Application.Errors;
using BookStore.Catalog.Application.Interfaces;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Query;

public class GetBookByIdQueryHandler
    : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    private readonly IBookRepository _repo;
    private readonly IBookCacheService _cache;

    public GetBookByIdQueryHandler(
        IBookRepository repo,
        IBookCacheService cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<BookDto>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(request.Id);

        if (cached is not null)
        {
            return Result.Success(new BookDto
            {
                Id = cached.Id,
                Title = cached.Title,
                Author = cached.Author,
                Price = cached.Price,
                Stock = cached.Stock
            });
        }

        var book = await _repo.GetByIdAsync(request.Id);

        if (book is null)
        {
            return Result.Failure<BookDto>(
                CatalogErrors.BookNotFound(request.Id));
        }

        await _cache.SetAsync(book);

        return Result.Success(new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            Stock = book.Stock
        });
    }
}