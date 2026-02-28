using Mediator;

namespace TbdDevelop.Mediator.Outbox.Postgres.Tests.Models;

public class SampleErrorEventType : INotification
{
    public string Content { get; set; } = "I've had enough, I'm off!";
}