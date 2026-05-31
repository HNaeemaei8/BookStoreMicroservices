using BookStore.Catalog.Application.Interfaces;
using BookStore.Catalog.Domain.Entities;
using BookStore.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Catalog.Infrastructure.Repository;

public class BookRepository : IBookRepository
{
    private readonly CatalogDbContext _dbContext;

    public BookRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Books
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Book book)
    {
        await _dbContext.Books.AddAsync(book);
    }

    public void Update(Book book)
    {
        _dbContext.Books.Update(book);
    }

    public void Delete(Book book)
    {
        _dbContext.Books.Remove(book);
    }

    public async Task<List<Book>> GetAllAsync()
    {
      return await _dbContext.Books.ToListAsync();
    }
}