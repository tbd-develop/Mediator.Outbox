using Mediator;
using TbdDevelop.Mediator.Outbox.Infrastructure;

namespace TbdDevelop.Mediator.Outbox.Outbox;

public class DefaultOutboxMessage
{
    public int Retries { get; set; }
}

public class DefaultOutboxMessage<TEvent>(Guid id, DateTime dateAdded, TEvent @event) :
    DefaultOutboxMessage,
    IOutboxMessage<Guid, TEvent> where TEvent : INotification
{
    public DateTime DateAdded { get; } = dateAdded;
    public Guid Id { get; } = id;
    public TEvent Event { get; } = @event;
}