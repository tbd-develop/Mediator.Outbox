using Mediator;

namespace TbdDevelop.Mediator.Outbox.Infrastructure;

public interface INotificationPublisher
{
    ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;

    ValueTask Publish(object notification, CancellationToken cancellationToken = default);
}