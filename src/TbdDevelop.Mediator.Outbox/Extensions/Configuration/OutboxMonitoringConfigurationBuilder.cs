using Microsoft.Extensions.DependencyInjection;

namespace TbdDevelop.Mediator.Outbox.Extensions.Configuration;

public class OutboxMonitoringConfigurationBuilder(IServiceCollection services)
{
    public OutboxMonitoringConfigurationBuilder WithSettings(Action<OutboxMonitoringConfiguration> configure)
    {
        services.Configure(configure);

        return this;
    }
}