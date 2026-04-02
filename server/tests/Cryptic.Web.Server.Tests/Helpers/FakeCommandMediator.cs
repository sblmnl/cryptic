using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;

namespace Cryptic.Web.Server.Tests.Helpers;

internal class FakeCommandMediator : ICommandMediator
{
    private readonly Dictionary<Type, Func<object, CancellationToken, Task<object>>> _handlers = new();
    private readonly List<object> _sentCommands = new();

    public IReadOnlyList<object> SentCommands => _sentCommands;

    public void Setup<TCommand, TResult>(Func<TCommand, TResult> handler) where TCommand : ICommand<TResult>
    {
        _handlers[typeof(TCommand)] = (cmd, _) => Task.FromResult<object>(handler((TCommand)cmd)!);
    }

    public Task<TCommandResult> SendAsync<TCommandResult>(
        ICommand<TCommandResult> command,
        CommandMediationSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        _sentCommands.Add(command);
        var commandType = command.GetType();

        if (_handlers.TryGetValue(commandType, out var handler))
        {
            return handler(command, cancellationToken).ContinueWith(t => (TCommandResult)t.Result);
        }

        throw new InvalidOperationException($"No handler registered for {commandType.Name}");
    }

    public Task SendAsync(
        ICommand command,
        CommandMediationSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        _sentCommands.Add(command);
        var commandType = command.GetType();

        if (_handlers.TryGetValue(commandType, out var handler))
        {
            return handler(command, cancellationToken);
        }

        throw new InvalidOperationException($"No handler registered for {commandType.Name}");
    }

    public T? GetSentCommand<T>() where T : class => _sentCommands.OfType<T>().LastOrDefault();
}
