using Mediator;
using TbdDevelop.Mediator.Outbox.Contracts;

namespace TbdDevelop.Mediator.Outbox.SqlServer;

public class SqlOutboxMessage<TEvent>(int id, int retries, DateTime dateAdded, TEvent @event) : IOutboxMessage<int, TEvent>
    where TEvent : class, INotification
{
    public int Id { get; } = id;
    public int Retries { get; } = retries;
    public DateTime DateAdded { get; } = dateAdded;
    public TEvent Event { get; } = @event;
}