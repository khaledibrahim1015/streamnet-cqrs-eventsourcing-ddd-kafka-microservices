using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsWithAuthorQuery : BaseQuery
    {
        public string Author { get; set; }
    }
}
