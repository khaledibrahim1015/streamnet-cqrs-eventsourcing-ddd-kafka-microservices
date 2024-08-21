using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    public Guid Id
    {
        get { return _id; }
        set { _id = value; }
    }
    private readonly List<BaseEvent> _changes = new List<BaseEvent>();
    public int Version = -1;


    public IEnumerable<BaseEvent> GetUnCommitedChanges() => _changes;
    public void MarkChangesAsCommited() => _changes.Clear();

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        //  get instance of concrete aggregateRoot (PostAggregate) (Reflication
        var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
        if (method == null)
            throw new ArgumentNullException(nameof(method), $"the apply method was not found in the aggregate for{@event.GetType().Name}");

        method.Invoke(this, new object[] { @event });

        if (isNew)
            _changes.Add(@event);
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChange(@event, false);
        }
    }

}
