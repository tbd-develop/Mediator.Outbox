using Mediator;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Tests.Models;

public class SampleEventType : INotification
{
    public string Content { get; set; } = "Hello, World";
}