namespace Shared.Contracts.Events
{
    public record StockReservedEvent(
        Guid OrderId,
        Guid CorrelationId);
}
