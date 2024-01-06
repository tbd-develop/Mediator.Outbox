using Mediator;

namespace TbdDevelop.Mediator.Outbox.Contracts;

public interface IOutboxMessage
{
    object Id { get; }
    object Event { get; }
}

public interface IOutboxMessage<out TKey, out TEvent> : IOutboxMessage
    where TEvent : class, INotification
{
    new TKey Id { get; }

    TEvent Event { get; }

    DateTime DateAdded { get; }

    object IOutboxMessage.Id => Id;
    object IOutboxMessage.Event => Event;
}