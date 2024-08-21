using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourceHandler : IEventSourceHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;

    public EventSourceHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<PostAggregate> GetById(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventAsync(aggregateId);

        if (events == null || !events.Any())
            return aggregate;
        aggregate.ReplayEvents(events);
        var latestVersion = events.Select(e => e.Version).Max();
        aggregate.Version = latestVersion;
        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUnCommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommited();
    }
}
