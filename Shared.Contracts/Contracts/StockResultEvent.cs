
namespace Shared.Contracts.Events;

public class StockResultEvent()
{
    public Guid OrderId { get; set; }
    public bool IsSuccess { get; set; }
    public string? Reason { get; set; }
}
