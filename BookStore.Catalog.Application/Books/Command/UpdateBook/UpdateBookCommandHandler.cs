using BookStore.Catalog.Application.Errors;
using BookStore.Catalog.Application.Interfaces;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Command;

public class UpdateBookCommandHandler
    : IRequestHandler<UpdateBookCommand, Result>
{
    private readonly IBookRepository _repo;
    private readonly IBookCacheService _cache;
    private readonly IUnitOfWork _uow;

    public UpdateBookCommandHandler(
        IBookRepository repo,
        IBookCacheService cache,
        IUnitOfWork uow)
    {
        _repo = repo;
        _cache = cache;
        _uow = uow;
    }

    public async Task<Result> Handle(
        UpdateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _repo.GetByIdAsync(request.Id);

        if (book is null)
            return Result.Failure(
                CatalogErrors.BookNotFound(request.Id));

        book.Title = request.Title;
        book.Author = request.Author;
        book.Price = request.Price;
        book.Stock = request.Stock;

        _repo.Update(book);

        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(book.Id);

        return Result.Success();
    }
}