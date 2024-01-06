using System.Reflection;
using Mediator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TbdDevelop.Mediator.Outbox.Contracts;

namespace TbdDevelop.Mediator.Outbox.Services;

public class OutboxMonitoringService(
    IOutboxStorage storage,
    ILogger<OutboxMonitoringService> logger,
    IPublisher publisher) : BackgroundService
{
    private const int DefaultDelayTimeMs = 25;
    private const int BackoffIntervalMs = 1000;
    private const int MaxBackOffIntervalMs = 10000;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delayTimeMs = DefaultDelayTimeMs;

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

                    delayTimeMs = DefaultDelayTimeMs;
                }
                catch (Exception exception)
                {
                    if (delayTimeMs < MaxBackOffIntervalMs)
                    {
                        delayTimeMs += BackoffIntervalMs;
                    }

                    logger.LogError(exception, "Error while publishing message. Backing off for {BackoffIntervalMs}ms",
                        BackoffIntervalMs);
                }

                await Task.Delay(delayTimeMs, stoppingToken);
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