using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository : ICommenRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task CreateCommentAsync(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Add(comment);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(Guid commentId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var commnt = await GetByIdAsync(commentId);
        if (commnt != null)
        {
            context.Comments.Remove(commnt);
            _ = await context.SaveChangesAsync();

        }
        return;
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId);
    }

    public async Task UpdateCommentAsync(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Update(comment);
        _ = await context.SaveChangesAsync();
    }
}
