using Mediator;

namespace TbdDevelop.Mediator.Outbox.Tests.Supporting;

public class SampleNotification : INotification
{
    public string Name { get; set; } = null!;
}