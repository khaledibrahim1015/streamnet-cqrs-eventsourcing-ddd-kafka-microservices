﻿using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreatePostAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Add(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(Guid postId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var post = await GetPostByIdAsync(postId);
        if (post != null)
        {
            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();

        }
        return;

    }
    public async Task UpdatePostAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Update(post);
        _ = await context.SaveChangesAsync();
    }
    public async Task<PostEntity> GetPostByIdAsync(Guid postId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(x => x.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
                    .Include(x => x.Comments).AsNoTracking()
                    .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
                    .Include(x => x.Comments).AsNoTracking()
                    .Where(p => p.Author.Contains(author))
                    .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByLikesAsync(int numberOfLikes)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
                    .Include(x => x.Comments).AsNoTracking()
                    .Where(p => p.Likes >= numberOfLikes)
                    .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
                    .Include(x => x.Comments).AsNoTracking()
                    .Where(p => p.Comments != null && p.Comments.Any())
                    .ToListAsync();
    }


}
