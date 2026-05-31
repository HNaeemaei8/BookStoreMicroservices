public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrderId { get; set; }

    public Guid BookId { get;  set; }

    public int Quantity { get;  set; }

    public decimal UnitPrice { get;  set; }

    public decimal TotalPrice => Quantity * UnitPrice;

    public OrderItem() { }

    public OrderItem(Guid bookId, int quantity, decimal unitPrice)
    {
        BookId = bookId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}