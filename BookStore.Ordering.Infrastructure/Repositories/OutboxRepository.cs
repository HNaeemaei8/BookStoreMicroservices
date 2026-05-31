using BookStore.Ordering.Infrastructure.Persistence;
using Domain.Entities.IntegrationEventLog;
using Microsoft.EntityFrameworkCore;


    public class OutboxRepository : IOutboxRepository
    {
        private readonly OrderDbContext _dbContext;

        public OutboxRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(IntegrationEventLog eventLog)
        {
            _dbContext.IntegrationEventLogs.Add(eventLog);
        }

        public async Task<List<IntegrationEventLog>> GetPendingEventsAsync(int count)
        {
            return await _dbContext.IntegrationEventLogs
                .Where(x => x.Status == EventStatus.Pending)
                .OrderBy(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public void MarkAsPublished(IntegrationEventLog eventLog)
        {
            eventLog.Status = EventStatus.Published;
        }

    public void MarkAsProcessing(IntegrationEventLog eventLog)
    {
        eventLog.Status = EventStatus.Processing;
    }

    public void MarkAsFailed(IntegrationEventLog eventLog)
    {
        eventLog.Status = EventStatus.Failed;
    }
}
