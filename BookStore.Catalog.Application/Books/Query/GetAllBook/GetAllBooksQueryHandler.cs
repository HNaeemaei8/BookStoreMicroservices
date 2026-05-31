using BookStore.Catalog.Application.Books.Dtos;
using BookStore.Catalog.Application.Interfaces;
using BookStore.Shared.Common.Results;
using MediatR;
namespace BookStore.Catalog.Application.Books.Query;

public class GetAllBooksQueryHandler
    : IRequestHandler<GetAllBooksQuery, Result<List<BookDto>>>
{
    private readonly IBookRepository _repo;

    public GetAllBooksQueryHandler(
        IBookRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<BookDto>>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        var books = await _repo.GetAllAsync();

        var result = books.Select(x => new BookDto
        {
            Id = x.Id,
            Title = x.Title,
            Author = x.Author,
            Price = x.Price,
            Stock = x.Stock
        }).ToList();

        return Result.Success(result);
    }
}