using Mediator;

namespace TbdDevelop.Mediator.Outbox.Contracts;

public interface IOutboxMessage
{
    object Id { get; }
    object Event { get; }
    int Retries { get; }

    DateTime DateAdded { get; }
}

public interface IOutboxMessage<out TKey, out TEvent> : IOutboxMessage
    where TEvent : INotification
{
    TKey Id { get; }
    TEvent Event { get; }

    object IOutboxMessage.Id => Id!;
    object IOutboxMessage.Event => Event;
}