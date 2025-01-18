using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TbdDevelop.Mediator.Outbox.Contracts;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;

namespace TbdDevelop.Mediator.Outbox.Services;

public class OutboxMonitoringService(
    IOutboxStorage storage,
    ILogger<OutboxMonitoringService> logger,
    IOutboxProcessingPublisher processingPublisher,
    IOptions<OutboxMonitoringConfiguration> options) : BackgroundService
{
    private readonly Lazy<OutboxMonitoringConfiguration> _configuration = new(() => options.Value);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            do
            {
                if (await ProcessNextOutboxQueueMessage(stoppingToken) != QueueStatus.Continue)
                {
                    logger.LogError("Error while publishing message. Shutting down service");

                    break;
                }

                await Task.Delay(_configuration.Value.Interval, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested);
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
    }

    private async Task<QueueStatus> ProcessNextOutboxQueueMessage(CancellationToken cancellationToken)
    {
        var message = await storage.RetrieveNextMessage(cancellationToken);

        if (message is null || await TryPublishMessage(message, cancellationToken))
        {
            return QueueStatus.Continue;
        }

        return await HandleMessageFailure(message, cancellationToken);
    }

    private async Task<QueueStatus> HandleMessageFailure(IOutboxMessage message, CancellationToken cancellationToken)
    {
        if (_configuration.Value.ShutdownOnException)
        {
            return QueueStatus.Shutdown;
        }

        if (ShouldRetryMessage(message))
        {
            await storage.IncreaseRetryCount(message, cancellationToken);
        }
        else
        {
            await storage.MoveToDeadLetterQueue(message, cancellationToken);
        }

        return QueueStatus.Continue;
    }

    private bool ShouldRetryMessage(IOutboxMessage message)
    {
        return message.Retries < _configuration.Value.MaximumRetryCount;
    }

    private async Task<bool> TryPublishMessage(IOutboxMessage message, CancellationToken stoppingToken)
    {
        try
        {
            await PublishMessage(message, stoppingToken);

            await storage.Commit(message, stoppingToken);

            return true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while publishing message {messageId}", message?.Id);
        }

        return false;
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

        await (Task)genericMethod.Invoke(this, [message.Event, cancellationToken])!;
    }

    private async Task PublishEvent<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class
    {
        await processingPublisher.Publish(@event, cancellationToken);
    }
}