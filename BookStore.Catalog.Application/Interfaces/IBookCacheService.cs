
using BookStore.Catalog.Domain.Entities;

namespace BookStore.Catalog.Application.Interfaces;

    public interface IBookCacheService
    {
        Task<Book?> GetAsync(Guid id);
        Task SetAsync(Book book);
        Task RemoveAsync(Guid id);
    }

