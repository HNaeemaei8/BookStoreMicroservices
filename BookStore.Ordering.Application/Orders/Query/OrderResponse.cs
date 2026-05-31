public class OrderResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemResponse> Items { get; set; }
}

public class OrderItemResponse
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}