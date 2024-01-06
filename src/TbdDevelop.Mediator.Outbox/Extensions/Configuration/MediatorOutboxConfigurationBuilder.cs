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

    public MediatorOutboxConfigurationBuilder UseInMemoryOutbox()
    {
        _services.AddTransient<IOutbox, InMemoryOutboxStorage>();

        return this;
    }

    public MediatorOutboxConfigurationBuilder AddOutboxMonitoringService()
    {
        _services.AddHostedService<OutboxMonitoringService>();

        return this;
    }

    public MediatorOutboxConfigurationBuilder Register(Action<IServiceCollection> configure)
    {
        configure(_services);

        return this;
    }
}