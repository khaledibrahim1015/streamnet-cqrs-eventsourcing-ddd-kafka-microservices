using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new Dictionary<Type, Func<BaseCommand, Task>>();
    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.TryGetValue(typeof(T), out var commandhandler))
            throw new IndexOutOfRangeException($" You Can NOT Register The Same Command Handler Twice :{commandhandler}");
        _handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task> handler))
        {
            //  invoke function 
            await handler(command);
        }
        else
        {
            throw new ArgumentNullException(nameof(handler), " BNo command handler was registered !");
        }
    }
}
