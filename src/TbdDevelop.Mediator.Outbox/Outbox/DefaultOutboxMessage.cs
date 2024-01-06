using Mediator;
using TbdDevelop.Mediator.Outbox.Contracts;

namespace TbdDevelop.Mediator.Outbox.Outbox;

public class DefaultOutboxMessage<TEvent>(Guid id, DateTime dateAdded, TEvent @event) :
    IOutboxMessage<Guid, TEvent> where TEvent : INotification
{
    public DateTime DateAdded { get; } = dateAdded;
    public Guid Id { get; } = id;
    public TEvent Event { get; } = @event;
}