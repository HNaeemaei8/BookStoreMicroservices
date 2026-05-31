using BookStore.Ordering.Domain.Enums;

public class Order
{
    public Guid Id { get;  set; } = Guid.NewGuid();

    public List<OrderItem> Items { get;  set; } = new();

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal TotalPrice { get;  set; }

    public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;

    public Order() { }

    public Order(List<OrderItem> items)
    {
        Items = items;
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        TotalPrice = Items.Sum(x => x.TotalPrice);
    }
}