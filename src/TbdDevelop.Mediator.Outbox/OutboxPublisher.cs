using Mediator;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using INotificationPublisher = TbdDevelop.Mediator.Outbox.Infrastructure.INotificationPublisher;

namespace TbdDevelop.Mediator.Outbox;

public class OutboxPublisher(IOutbox outbox) : INotificationPublisher
{
    public ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new())
        where TNotification : INotification
    {
        return outbox.Add(notification, cancellationToken);
    }

    public ValueTask Publish(object notification, CancellationToken cancellationToken = default)
    {
        if (notification is INotification outboxNotification)
        {
            return Publish(outboxNotification, cancellationToken);
        }

        return ValueTask.CompletedTask;
    }
}