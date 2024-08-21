using CQRS.Core.Handlers;
using Post.Cmd.Api.Commands.interfaces;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourceHandler<PostAggregate> _eventSourceHandler;

        public CommandHandler(IEventSourceHandler<PostAggregate> eventSourceHandler)
        {
            _eventSourceHandler = eventSourceHandler;
        }

        public async Task HandleAsync(NewPostCommand command)
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
            await _eventSourceHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditMessageCommand command)
        {
            var aggregate = await _eventSourceHandler.GetById(command.Id);
            aggregate.EditMessage(command.Message);
            await _eventSourceHandler.SaveAsync(aggregate);

        }

        public async Task HandleAsync(LikePostCommand command)
        {
            var aggregate = await _eventSourceHandler.GetById(command.Id);
            aggregate.LikePost();
            await _eventSourceHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(AddCommentCommand command)
        {
            var aggregate = await _eventSourceHandler.GetById(command.Id);
            aggregate.AddComment(command.Comment, command.Username);
            await _eventSourceHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditCommentCommand command)
        {
            var aggregate = await _eventSourceHandler.GetById(command.Id);
            aggregate.EditComment(command.CommentId, command.Comment, command.Username);
            await _eventSourceHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(RemoveCommentCommand command)
        {
            var aggregate = await _eventSourceHandler.GetById(command.Id);
            aggregate.RemoveComment(command.CommentId, command.Username);
            await _eventSourceHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(DeletePostCommand command)
        {
            var aggregate = await _eventSourceHandler.GetById(command.Id);
            aggregate.DeletePost(command.Username);
            await _eventSourceHandler.SaveAsync(aggregate);
        }
    }
}
