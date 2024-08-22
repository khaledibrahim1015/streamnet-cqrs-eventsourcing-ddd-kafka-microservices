using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface ICommenRepository
{

    Task CreateCommentAsync(CommentEntity comment);
    Task UpdateCommentAsync(CommentEntity comment);
    Task DeleteCommentAsync(Guid commentId);
    Task<CommentEntity> GetByIdAsync(Guid commentId);

}
