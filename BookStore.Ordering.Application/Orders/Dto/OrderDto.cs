namespace BookStore.Ordering.Application.Orders.Dtos;

public class OrderDto
{
    public Guid Id { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<OrderItemDto> Items { get; set; } = new();
}