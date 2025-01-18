namespace TbdDevelop.Mediator.Outbox.Extensions.Configuration;

public class OutboxMonitoringConfiguration
{
    /// <summary>
    /// Interval between checks for new notifications
    /// Default: 5 seconds
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum number of times to retry a message before moving it to the dead letter queue
    /// Default: 3
    /// </summary>
    public int MaximumRetryCount { get; set; } = 3;

    /// <summary>
    /// Rather than backing off and retrying, an exception will cause the service to shutdown and re-throw the exception
    /// Default: false
    /// </summary>
    public bool ShutdownOnException { get; set; } = false;
}