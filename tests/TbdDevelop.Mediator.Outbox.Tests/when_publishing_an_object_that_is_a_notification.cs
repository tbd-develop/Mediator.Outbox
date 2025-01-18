using Mediator;
using NSubstitute;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Tests.Supporting;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.Tests;

public class when_publishing_an_object_that_is_a_notification
{
    private IOutbox _outbox = null!;
    private OutboxPublisher _subject = null!;

    public when_publishing_an_object_that_is_a_notification()
    {
        Arrange();

        Act();
    }

    [Fact]
    public void notification_is_added_to_outbox()
    {
        _outbox
            .Received()
            .Add(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    private void Arrange()
    {
        _outbox = Substitute.For<IOutbox>();

        _subject = new OutboxPublisher(_outbox);
    }

    private void Act()
    {
        _subject.Publish(new SampleNotification { Name = "Valid Notification" });
    }
}