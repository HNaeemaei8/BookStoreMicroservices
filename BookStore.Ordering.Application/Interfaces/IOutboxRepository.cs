using Domain.Entities.IntegrationEventLog;


    public interface IOutboxRepository
    {
        void Add(IntegrationEventLog eventLog);

        Task<List<IntegrationEventLog>> GetPendingEventsAsync(int count);

        void MarkAsPublished(IntegrationEventLog eventLog);
       
    void MarkAsProcessing(IntegrationEventLog eventLog);

    void MarkAsFailed(IntegrationEventLog eventLog);


    }


