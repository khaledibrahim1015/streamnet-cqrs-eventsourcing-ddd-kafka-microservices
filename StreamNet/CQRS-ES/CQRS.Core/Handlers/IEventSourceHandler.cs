using CQRS.Core.Domain;

namespace CQRS.Core.Handlers;

public interface IEventSourceHandler<T> where T : class
{
    Task SaveAsync(AggregateRoot aggregate);
    Task<T> GetById(Guid aggregateId);
}
