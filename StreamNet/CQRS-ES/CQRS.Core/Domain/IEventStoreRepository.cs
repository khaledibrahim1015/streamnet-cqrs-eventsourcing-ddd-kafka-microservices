﻿using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventStoreRepository
{

    Task SaveAsync(EventModel @event);
    Task<IEnumerable<EventModel>> FinfByAggregateId(Guid aggregateId);
}
