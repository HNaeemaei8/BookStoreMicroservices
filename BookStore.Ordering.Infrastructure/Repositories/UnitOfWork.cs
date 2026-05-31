using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Infrastructure.Persistence;
namespace BookStore.Ordering.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _dbContext;
    public UnitOfWork(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}