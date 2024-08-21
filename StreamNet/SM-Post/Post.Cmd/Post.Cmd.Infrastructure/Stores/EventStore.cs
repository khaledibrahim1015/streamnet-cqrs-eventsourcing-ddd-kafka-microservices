using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;
using System.Data;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public EventStore(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task<List<BaseEvent>> GetEventAsync(Guid aggregateId)
    {
        IEnumerable<EventModel> eventStream = await _eventStoreRepository.FinfByAggregateId(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundExcepion("Incorrect post id provided !");
        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();

    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FinfByAggregateId(aggregateId);
        //var eventStreamList = eventStream.ToList();
        //  access the last event's version in the stream.
        //if (expectedVersion != -1 && eventStreamList[^1].Version != expectedVersion)

        EventModel lastEvent = eventStream.Last();
        if (expectedVersion != -1 && lastEvent.Version != expectedVersion) //  if lastevent version not equal to expectedversion throw exception
            throw new ConcurrencyException();

        var version = expectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel()
            {
                TimeStamp = DateTime.Now,
                Version = version,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                EventType = eventType,
                EventData = @event,
            };

            await _eventStoreRepository.SaveAsync(eventModel);

        }

    }
}
