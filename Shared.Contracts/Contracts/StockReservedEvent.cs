namespace Shared.Contracts.Events;

public class StockReservedEvent
{
    public Guid OrderId { get; set; }
    public DateTime ReservedAt { get; set; }
}