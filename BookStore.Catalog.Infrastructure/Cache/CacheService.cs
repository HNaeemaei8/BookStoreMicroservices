using BookStore.Catalog.Application.Interfaces;
using BookStore.Catalog.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;
namespace BookStore.Catalog.Infrastructure.Cache
{
    public class BookCacheService : IBookCacheService
    {
        private readonly IDatabase _db;

        public BookCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<Book?> GetAsync(Guid id)
        {
            var data = await _db.StringGetAsync(id.ToString());
            return data.IsNull ? null : JsonSerializer.Deserialize<Book>(data!);
        }

        public async Task SetAsync(Book book)
        {
            var json = JsonSerializer.Serialize(book);
            await _db.StringSetAsync(book.Id.ToString(), json, TimeSpan.FromMinutes(5));
        }

        public async Task RemoveAsync(Guid id)
        {
            await _db.KeyDeleteAsync(id.ToString());
        }
    }
}
