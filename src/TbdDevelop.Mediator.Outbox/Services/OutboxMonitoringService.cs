using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;
using TbdDevelop.Mediator.Outbox.Infrastructure;

namespace TbdDevelop.Mediator.Outbox.Services;

public class OutboxMonitoringService(
    IQueueProcessor processor,
    ILogger<OutboxMonitoringService> logger,
    IOptions<OutboxMonitoringConfiguration> options
) : BackgroundService
{
    private readonly Lazy<OutboxMonitoringConfiguration> _configuration = new(() => options.Value);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            do
            {
                if (await processor.ProcessNextOutboxQueueMessage(stoppingToken) != QueueStatus.Continue)
                {
                    logger.LogError("Error while publishing message. Shutting down service");

                    break;
                }

                await Task.Delay(_configuration.Value.Interval, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested);
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
    }
}