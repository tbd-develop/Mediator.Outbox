using System.Reflection;
using Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TbdDevelop.Mediator.Outbox.Contracts;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;

namespace TbdDevelop.Mediator.Outbox.Services;

public class OutboxMonitoringService(
    IOutboxStorage storage,
    ILogger<OutboxMonitoringService> logger,
    IPublisher publisher,
    IOptions<OutboxMonitoringConfiguration> options) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configuration = options.Value;

        var delayTime = configuration.Interval;

        return Task.Factory.StartNew(async () =>
        {
            do
            {
                try
                {
                    var message = await storage.RetrieveNextMessage(stoppingToken);

                    if (message is null)
                        continue;

                    await PublishMessage(message, stoppingToken);

                    await storage.Commit(message, stoppingToken);

                    delayTime = configuration.Interval;
                }
                catch (Exception exception)
                {
                    if (!configuration.ShutdownOnException)
                    {
                        if (delayTime < configuration.MaximumBackOff)
                        {
                            delayTime += configuration.BackOffOnException;
                        }

                        logger.LogError(exception,
                            "Error while publishing message. Backing off for {BackoffIntervalMs}ms",
                            delayTime);
                    }
                    else
                    {
                        logger.LogError(exception,
                            "Error while publishing message. Shutting down service");

                        throw;
                    }
                }

                await Task.Delay(delayTime, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested);
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
    }

    private async Task PublishMessage(IOutboxMessage message, CancellationToken cancellationToken)
    {
        var type = message.Event.GetType();

        var method =
            typeof(OutboxMonitoringService).GetMethod(nameof(PublishEvent),
                BindingFlags.NonPublic | BindingFlags.Instance);

        if (method is null)
        {
            throw new Exception("Unable to publish message");
        }

        var genericMethod = method.MakeGenericMethod(type);

        await (Task)genericMethod.Invoke(this, new[] { message.Event, cancellationToken })!;
    }

    private async Task PublishEvent<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class, INotification
    {
        await publisher.Publish(@event, cancellationToken);
    }
}