using Mediator;
using NSubstitute;
using TbdDevelop.Mediator.Outbox.Contracts;
using TbdDevelop.Mediator.Outbox.Tests.Supporting;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.Tests;

public class when_publishing_an_object_that_is_a_notification
{
    private IOutbox Outbox = null!;
    private OutboxPublisher OutboxPublisher = null!;

    public when_publishing_an_object_that_is_a_notification()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void notification_is_added_to_outbox()
    {
        Outbox
            .Received()
            .Add(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    private void Arrange()
    {
        Outbox = Substitute.For<IOutbox>();

        OutboxPublisher = new OutboxPublisher(Outbox);
    }

    private void Act()
    {
        OutboxPublisher.Publish(new SampleNotification { Name = "Valid Notification" });
    }
}