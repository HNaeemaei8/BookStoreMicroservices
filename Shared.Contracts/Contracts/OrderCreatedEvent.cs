namespace Shared.Contracts.Events
{
    public record OrderCreatedEvent(Guid OrderId, Guid BookId, int Quantity)
    {
        public Guid CorrelationId { get; set; }
    }
   
}
