using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Contracts;

namespace TbdDevelop.Mediator.Outbox.Extensions.Configuration;

public class OutboxMonitoringConfigurationBuilder(IServiceCollection services)
{
    public OutboxMonitoringConfigurationBuilder WithPublisher<TPublisher>()
        where TPublisher : class, IOutboxProcessingPublisher
    {
        services.AddSingleton<IOutboxProcessingPublisher, TPublisher>();

        return this;
    }

    /// <summary>
    /// Configure the settings for the monitoring. Interval, Maximum Retries etc.
    /// </summary>
    /// <param name="configure"></param>
    /// <returns>OutboxMonitoringConfigurationBuilder</returns>
    public OutboxMonitoringConfigurationBuilder WithSettings(Action<OutboxMonitoringConfiguration> configure)
    {
        services.Configure(configure);

        return this;
    }
}