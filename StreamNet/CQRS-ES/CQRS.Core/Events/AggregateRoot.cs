namespace CQRS.Core.Events;

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

}
