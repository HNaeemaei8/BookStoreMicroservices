namespace Domain.Entities.IntegrationEventLog;
public class IntegrationEventLog
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string EventData { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Pending;

    public int RetryCount { get; set; }

    public void MarkAsPublished()
    {
        Status = EventStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = EventStatus.Failed;
        RetryCount++;
    }
}
public enum EventStatus
{
    Pending,
    Processing,
    Published,
    Failed
}