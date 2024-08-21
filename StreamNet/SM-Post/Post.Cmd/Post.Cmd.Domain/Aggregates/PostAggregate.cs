﻿using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;

    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public string Author
    {
        get => _author;
        set => _author = value;
    }
    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent()
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now,

        });

    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!_active)
            throw new InvalidOperationException("you can not edit message of an active post !");
        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException($"the value of {nameof(message)} can not be null or empty please provide a valid {nameof(message)} !");

        RaiseEvent(new MessageUpdatedEvent()
        {
            Id = _id,
            Message = message,

        });

    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
            throw new InvalidOperationException("you can not Like of an inactive post !");
        RaiseEvent(new PostLikeEvent()
        {
            Id = _id,
        });

    }
    public void Apply(PostLikeEvent @event)
    {
        _id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!_active) throw new InvalidOperationException("you can not add a comment of an inactive post !");

        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException($"the value of {nameof(comment)} can not be null or empty please provide a valid {nameof(comment)} !");
        RaiseEvent(new CommentAddedEvent()
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            CommentDate = DateTime.Now,
            Comment = comment,
            Username = username
        });
    }
    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active) throw new InvalidOperationException("you can not Edit  a comment of an inactive post !");
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException(" you are not allowed to edit a comment that was made by another user !");
        RaiseEvent(new CommentUpdatedEvent()
        {
            Id = _id,
            CommentId = commentId,
            EditDate = DateTime.Now,
            Comment = comment,
            Username = username
        });
    }
    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active) throw new InvalidOperationException("you can not remove a comment of an inactive post !");
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException(" you are not allowed to remove a comment that was made by another user !");

        RaiseEvent(new CommentRemovedEvent()
        {
            Id = _id,
            CommentId = commentId,

        });

    }
    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active) throw new InvalidOperationException("the post has already been removed !");
        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("you are ot allowed to delete a post that was made by someone else !");

        RaiseEvent(new PostRemovedEvent()
        {
            Id = _id,

        });
    }
    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }

}
