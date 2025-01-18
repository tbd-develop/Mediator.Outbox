using Mediator;
using TbdDevelop.Mediator.Outbox.Infrastructure;

namespace TbdDevelop.Mediator.Outbox.Outbox;

public class DefaultOutboxProcessingPublisher(IPublisher publisher) : IOutboxProcessingPublisher
{
    public ValueTask Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        return publisher.Publish(@event, cancellationToken);
    }
}