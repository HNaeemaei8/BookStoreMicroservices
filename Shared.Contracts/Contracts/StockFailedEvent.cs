namespace Shared.Contracts.Events
{
    public record StockFailedEvent(
        Guid OrderId,
        string Reason,
        Guid CorrelationId);
}
