using BookStore.Ordering.Application.Interfaces;
using BookStore.Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Ordering.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }

}