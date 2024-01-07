using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Contracts;
using TbdDevelop.Mediator.Outbox.Outbox;
using TbdDevelop.Mediator.Outbox.Services;

namespace TbdDevelop.Mediator.Outbox.Extensions.Configuration;

public class MediatorOutboxConfigurationBuilder
{
    private readonly IServiceCollection _services;

    public MediatorOutboxConfigurationBuilder(IServiceCollection services)
    {
        _services = services;

        services.AddTransient<INotificationPublisher, OutboxPublisher>();
    }

    /// <summary>
    /// Use an in-memory outbox storage.  This is not recommended for production use.
    /// </summary>
    /// <returns></returns>
    public MediatorOutboxConfigurationBuilder UseInMemoryOutbox()
    {
        _services.AddTransient<IOutbox, InMemoryOutboxStorage>();

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
        Action<OutboxMonitoringConfigurationBuilder>? configure = null)
    {
        _services.AddHostedService<OutboxMonitoringService>();

        if (configure is null)
        {
            // Configure default options 

            _services.Configure<OutboxMonitoringConfiguration>(_ => { });

            return this;
        }

        var builder = new OutboxMonitoringConfigurationBuilder(_services);

        configure(builder);

        return this;
    }

    public MediatorOutboxConfigurationBuilder Register(Action<IServiceCollection> configure)
    {
        configure(_services);

        return this;
    }
}