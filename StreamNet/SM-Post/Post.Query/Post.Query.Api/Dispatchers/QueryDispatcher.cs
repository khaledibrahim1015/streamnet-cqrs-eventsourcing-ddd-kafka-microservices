using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<PostEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = new();

        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
                throw new InvalidOperationException($"You cannot register the same query handler twice: {typeof(TQuery).Name}");
            _handlers[typeof(TQuery)] = x => handler((TQuery)x);
        }

        public async Task<List<PostEntity>> SendAsync(BaseQuery query)
        {
            if (_handlers.TryGetValue(query.GetType(), out var handler))
            {
                return await handler(query);
            }
            throw new InvalidOperationException($"No query handler was registered for {query.GetType().Name}");
        }
    }
}