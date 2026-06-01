namespace Shared.Contracts.Events;

public class StockFailedEvent
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = default!;
    public DateTime FailedAt { get; set; }
}