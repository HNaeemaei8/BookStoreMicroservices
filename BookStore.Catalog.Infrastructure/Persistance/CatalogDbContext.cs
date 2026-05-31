using BookStore.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                  .HasMaxLength(250);

            entity.Property(x => x.Author)
                  .HasMaxLength(250);

            entity.Property(x => x.Price)
                  .HasColumnType("decimal(18,2)");
        });
    }
}