using CQRS.Core.Domain;
using CQRS.Core.Enums;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Configuraion;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoCollection<EventModel> _eventStoreCollection;
    private readonly IMongoCollection<OutboxMessage> _outboxCollection;
    public EventStoreRepository(IOptions<MongoDbConfiguration> config)
    {
        _mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = _mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        _outboxCollection = mongoDatabase.GetCollection<OutboxMessage>("Outbox");

    }

    public async Task<IEnumerable<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection
                  .Find(x => x.AggregateIdentifier == aggregateId)
                      .ToListAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel @event, OutboxMessage outboxMessage)
    {
        using var session = await _mongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            await _eventStoreCollection.InsertOneAsync(session, @event).ConfigureAwait(false);
            await _outboxCollection.InsertOneAsync(session, outboxMessage).ConfigureAwait(false);

            await session.CommitTransactionAsync();
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    //public async Task SaveAsync(EventModel @event)
    //{
    //    await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    //}


    public async Task<IEnumerable<OutboxMessage>> GetPendingOutboxMessagesAsync()
    {
        return await _outboxCollection
            .Find(x => x.Status == OutboxMessageStatus.Pending)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task MarkOutboxMessageAsProcessedAsync(Guid messageId)
    {
        var update = Builders<OutboxMessage>.Update.Set(x => x.Status, OutboxMessageStatus.Processed);
        await _outboxCollection.UpdateOneAsync(x => x.Id == messageId, update).ConfigureAwait(false);
    }



}
