using System.Collections.Concurrent;
using Mediator;
using TbdDevelop.Mediator.Outbox.Contracts;

namespace TbdDevelop.Mediator.Outbox.Outbox;

public class InMemoryOutboxStorage : IOutboxStorage, IOutbox
{
    private readonly ConcurrentDictionary<Guid, IOutboxMessage> _outbox = new();

    public async ValueTask Add<TNotification>(TNotification notification,
        CancellationToken cancellationToken = new()) where TNotification : INotification
    {
        await Task.Run(() =>
        {
            var message = new DefaultOutboxMessage<TNotification>(Guid.NewGuid(), DateTime.UtcNow, notification);

            _outbox.TryAdd(message.Id, message);
        }, cancellationToken);
    }

    public Task<IOutboxMessage?> RetrieveNextMessage(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var nextMessage = (from message in _outbox.Values
                orderby message.DateAdded
                select message).FirstOrDefault();

            return nextMessage;
        }, cancellationToken);
    }

    public Task Commit(IOutboxMessage message, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => { _outbox.TryRemove((Guid)message.Id, out _); }, cancellationToken);
    }

    public Task IncreaseRetryCount(IOutboxMessage message, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (!_outbox.TryGetValue((Guid)message.Id, out var outboxMessage))
            {
                return;
            }
            
            if (outboxMessage is DefaultOutboxMessage messageToRetry)
            {
                messageToRetry.Retries++;
            }
        }, cancellationToken);
    }

    public Task MoveToDeadLetterQueue(IOutboxMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}