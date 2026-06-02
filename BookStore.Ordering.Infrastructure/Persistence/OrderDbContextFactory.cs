using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookStore.Ordering.Infrastructure.Persistence;

public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=BookStoreOrderingDb;User Id=sa;Password=BookStore2025!;TrustServerCertificate=True");

        return new OrderDbContext(optionsBuilder.Options);
    }
}