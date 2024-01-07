namespace TbdDevelop.Mediator.Outbox.Extensions.Configuration;

public class OutboxMonitoringConfiguration
{
    /// <summary>
    /// Interval between checks for new notifications
    /// Default: 5 seconds
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// On exception, add to the interval between checks for new notifications
    /// Default: 5 seconds
    /// </summary>
    public TimeSpan BackOffOnException { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum interval between checks after adding back-offs 
    /// Default: 60 seconds
    /// </summary>
    public TimeSpan MaximumBackOff { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Rather than backing off and retrying, an exception will cause the service to shutdown and re-throw the exception
    /// Default: false
    /// </summary>
    public bool ShutdownOnException { get; set; } = false;
}