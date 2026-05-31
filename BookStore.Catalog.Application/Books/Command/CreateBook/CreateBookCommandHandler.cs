
using BookStore.Catalog.Application.Interfaces;
using BookStore.Catalog.Domain.Entities;
using BookStore.Shared.Common.Results;
using MediatR;

namespace BookStore.Catalog.Application.Command;

public class CreateBookCommandHandler
    : IRequestHandler<CreateBookCommand, Result<Guid>>
{
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(
        IBookRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Price = request.Price,
            Stock = request.Stock
        };

        await _repository.AddAsync(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(book.Id);
    }
}