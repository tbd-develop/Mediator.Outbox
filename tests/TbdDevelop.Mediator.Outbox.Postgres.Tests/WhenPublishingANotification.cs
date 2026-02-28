using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Postgres.Context;
using TbdDevelop.Mediator.Outbox.Postgres.Tests.Fixtures;
using TbdDevelop.Mediator.Outbox.Postgres.Tests.Models;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.Postgres.Tests;

public class WhenPublishingANotification(
    OutboxContextFixture fixture,
    ITestOutputHelper outputHelper)
    : IClassFixture<OutboxContextFixture>
{
    [Fact]
    public async Task notification_is_added_to_outbox()
    {
        var publisher = fixture.Provider.Value.GetRequiredService<INotificationPublisher>();
        var factory = fixture.Provider.Value.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

        outputHelper.WriteLine("Publishing Notification");

        await publisher.Publish(
            new SampleEventType
            {
                Content = "Message From Notification Publisher"
            },
            CancellationToken.None);

        await using var context = await factory.CreateDbContextAsync(CancellationToken.None);

        var outboxMessages = await context.OutboxMessages.ToListAsync(CancellationToken.None);

        outputHelper.WriteLine($"{outboxMessages.Count} messages found in outbox");

        Assert.NotEmpty(outboxMessages);
    }
}