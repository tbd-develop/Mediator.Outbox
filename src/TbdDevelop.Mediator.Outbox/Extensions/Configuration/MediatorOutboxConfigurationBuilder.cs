using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Outbox;
using TbdDevelop.Mediator.Outbox.Services;

namespace TbdDevelop.Mediator.Outbox.Extensions.Configuration;

public class MediatorOutboxConfigurationBuilder
{
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
    private readonly IMediatorServiceCollection _services;

    public MediatorOutboxConfigurationBuilder(
        IServiceCollection services
    )
    {
        _services = new MediatorServiceCollection(ServiceLifetime, services);

        _services.AddInServiceLifetime<INotificationPublisher, OutboxPublisher>();
    }

    /// <summary>
    /// Use an in-memory outbox storage.  This is not recommended for production use.
    /// </summary>
    /// <returns></returns>
    public MediatorOutboxConfigurationBuilder UseInMemoryOutbox()
    {
        _services.AddInServiceLifetime<IOutbox, InMemoryOutboxStorage>();

        return this;
    }

    /// <summary>
    /// Uses the IOutboxStorage to retrieve notifications and attempt to publish them.
    /// On exception, it will increase the time it waits before checking again up to a
    /// configurable maximum wait time. It will keep retrying indefinitely.
    ///  
    /// On success, it will commit the notification with IOutboxStorage.  
    /// </summary>
    /// <param name="configure">Optional override for configuring timeouts</param>
    /// <returns></returns>
    public MediatorOutboxConfigurationBuilder AddOutboxMonitoringService(
        Action<OutboxMonitoringConfigurationBuilder>? configure = null
    )
    {
        _services.AddHostedService<OutboxMonitoringService>();

        _services.AddInServiceLifetime<IQueueProcessor, OutboxMessageProcessor>();

        if ( configure is null )
        {
            _services.Configure<OutboxMonitoringConfiguration>(_ => { });

            return this;
        }

        var builder = new OutboxMonitoringConfigurationBuilder(_services);

        configure(builder);

        return this;
    }

    public MediatorOutboxConfigurationBuilder Register(
        Action<IMediatorServiceCollection> configure
    )
    {
        configure(_services);

        return this;
    }
}