using CQRS.Core.Queries;

namespace CQRS.Core.Infrastructure;

public interface IQueryDispatcher<TEntity> where TEntity : class
{
    void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery : BaseQuery;
    Task<List<TEntity>> SendAsync(BaseQuery query);
}
