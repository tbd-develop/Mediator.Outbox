namespace TbdDevelop.Mediator.Outbox.Infrastructure;

public interface IOutboxProcessingPublisher
{
    public ValueTask Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}