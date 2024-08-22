using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommenRepository _commenRepository;

    public EventHandler(IPostRepository postRepository, ICommenRepository commenRepository)
    {
        _postRepository = postRepository;
        _commenRepository = commenRepository;
    }

    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity()
        {
            PostId = @event.Id,
            Author = @event.Author,
            Message = @event.Message,
            DatePosted = @event.DatePosted,
        };
        await _postRepository.CreatePostAsync(post);
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await _postRepository.GetPostByIdAsync(@event.Id);

        if (post == null) return;

        post.Message = @event.Message;

        await _postRepository.UpdatePostAsync(post);
    }

    public async Task On(PostLikeEvent @event)
    {
        var post = await _postRepository.GetPostByIdAsync(@event.Id);

        if (post == null) return;

        post.Likes++;
        await _postRepository.UpdatePostAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity()
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            Comment = @event.Comment,
            CommentDate = @event.CommentDate,
            Username = @event.Username,
            IsEdited = false
        };
        await _commenRepository.CreateCommentAsync(comment);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await _commenRepository.GetByIdAsync(@event.CommentId);
        if (comment == null) return;
        comment.Comment = @event.Comment;
        comment.IsEdited = true;
        comment.CommentDate = @event.EditDate;

        await _commenRepository.UpdateCommentAsync(comment);
    }

    public async Task On(CommentRemovedEvent @event)
    {

        await _commenRepository.DeleteCommentAsync(@event.CommentId);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await _postRepository.DeletePostAsync(@event.Id);
    }
}
