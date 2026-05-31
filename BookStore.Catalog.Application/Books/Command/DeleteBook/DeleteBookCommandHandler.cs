using BookStore.Catalog.Application.Books.Command;
using BookStore.Catalog.Application.Errors;
using BookStore.Catalog.Application.Interfaces;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Books.Command;

public class DeleteBookCommandHandler
    : IRequestHandler<DeleteBookCommand, Result>
{
    private readonly IBookRepository _repo;
    private readonly IBookCacheService _cache;
    private readonly IUnitOfWork _uow;

    public DeleteBookCommandHandler(
        IBookRepository repo,
        IBookCacheService cache,
        IUnitOfWork uow)
    {
        _repo = repo;
        _cache = cache;
        _uow = uow;
    }

    public async Task<Result> Handle(
        DeleteBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _repo.GetByIdAsync(request.Id);

        if (book is null)
            return Result.Failure(
                CatalogErrors.BookNotFound(request.Id));

        _repo.Delete(book);

        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(book.Id);

        return Result.Success();
    }
}