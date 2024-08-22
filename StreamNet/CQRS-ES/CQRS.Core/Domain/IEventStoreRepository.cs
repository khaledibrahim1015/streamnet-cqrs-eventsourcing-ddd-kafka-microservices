using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventStoreRepository
{


    Task<IEnumerable<EventModel>> FindByAggregateId(Guid aggregateId);
    Task SaveAsync(EventModel @event, OutboxMessage outboxMessage);
    Task<IEnumerable<OutboxMessage>> GetPendingOutboxMessagesAsync();
    Task MarkOutboxMessageAsProcessedAsync(Guid messageId);
}
