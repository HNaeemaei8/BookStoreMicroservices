using BookStore.Catalog.Domain.Entities;

namespace BookStore.Catalog.Application.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<List<Book>> GetAllAsync();

    Task AddAsync(Book book);
    void Update(Book book);
    void Delete(Book book);
}