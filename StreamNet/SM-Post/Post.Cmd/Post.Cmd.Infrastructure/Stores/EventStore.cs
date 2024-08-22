using CQRS.Core.Domain;
using CQRS.Core.Enums;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System.Data;
using System.Text.Json;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task<List<BaseEvent>> GetEventAsync(Guid aggregateId)
    {
        IEnumerable<EventModel> eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundExcepion("Incorrect post id provided !");
        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();

    }


    // The problem:

    // Database operation succeeds
    // Kafka message production fails
    // implement the Outbox Pattern to solve classical problem in distributed  system 
    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
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

            //  Inject it in docker environment when build docker-compos and LaunchSettings.json
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Topic = topic,
                Payload = JsonSerializer.Serialize(@event),
                PayloadType = eventType,
                CreatedAt = DateTime.UtcNow,
                Status = OutboxMessageStatus.Pending
            };
            await _eventStoreRepository.SaveAsync(eventModel, outboxMessage);

        }

        // Trigger the outbox processor
        _ = Task.Run(() => ProcessOutboxMessagesAsync());
    }


    private async Task ProcessOutboxMessagesAsync()
    {
        var pendingMessages = await _eventStoreRepository.GetPendingOutboxMessagesAsync();
        foreach (var message in pendingMessages)
        {
            try
            {
                var eventType = Type.GetType(message.PayloadType);
                if (eventType == null)
                    throw new Exception($"Unknown event type: {message.PayloadType}");

                // Deserialize the payload to the correct type
                var eventData = JsonSerializer.Deserialize(message.Payload, eventType);
                if (eventData is BaseEvent baseEvent)
                    await _eventProducer.ProduceAsync(message.Topic, baseEvent);
                else
                    throw new Exception($"Deserialized object is not a BaseEvent: {message.PayloadType}");

                await _eventStoreRepository.MarkOutboxMessageAsProcessedAsync(message.Id);
            }
            catch (Exception ex)
            {
                // Log the error and continue processing other messages
                // You might want to implement a retry mechanism here
                Console.WriteLine($"Failed to process outbox message {message.Id}: {ex.Message}");
            }
        }
    }
}
