
using Domain.Entities.IntegrationEventLog;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Ordering.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity).IsRequired();

            entity.Property(x => x.UnitPrice)
                  .HasPrecision(18, 2);
        });


        modelBuilder.Entity<IntegrationEventLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(500);
            entity.Property(e => e.EventData).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                  .HasConversion<int>();

            entity.Property(x => x.TotalPrice)
                  .HasPrecision(18, 2);

            entity.Property(x => x.CreatedAt)
                  .IsRequired();

            entity.HasMany(x => x.Items)
                  .WithOne()
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });


    }
}