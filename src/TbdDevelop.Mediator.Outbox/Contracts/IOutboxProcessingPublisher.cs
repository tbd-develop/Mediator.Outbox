namespace TbdDevelop.Mediator.Outbox.Contracts;

public interface IOutboxProcessingPublisher
{
    public ValueTask Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}