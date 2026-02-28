using Mediator;

namespace TbdDevelop.Mediator.Outbox.Postgres.Tests.Models;

public class SampleEventType : INotification
{
    public string Content { get; set; } = "Hello, World";
}