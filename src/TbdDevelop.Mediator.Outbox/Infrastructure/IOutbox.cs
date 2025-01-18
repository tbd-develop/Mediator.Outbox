using Mediator;

namespace TbdDevelop.Mediator.Outbox.Infrastructure;

public interface IOutbox
{
    ValueTask Add<TNotification>(TNotification notification, CancellationToken cancellationToken = new())
        where TNotification : INotification;
}