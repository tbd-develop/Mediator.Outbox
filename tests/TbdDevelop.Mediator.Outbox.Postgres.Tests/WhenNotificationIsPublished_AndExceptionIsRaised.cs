using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Postgres.Context;
using TbdDevelop.Mediator.Outbox.Postgres.Tests.Fixtures;
using TbdDevelop.Mediator.Outbox.Postgres.Tests.Models;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.Postgres.Tests;

public class WhenNotificationIsPublished_AndExceptionIsRaised(
    OutboxContextFixture fixture,
    ITestOutputHelper outputHelper)
    : IClassFixture<OutboxContextFixture>
{
    [Fact]
    public async Task retry_count_is_increased()
    {
        await using (fixture.RedirectOutput(outputHelper)) ;

        // Arrange 

        using (var scope = fixture.Provider.Value.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);

            context.OutboxMessages.Add(
                SampleEventBuilder<SampleErrorEventType>.AsOutboxMessage(
                    msg => { msg.Content = "Pending Message"; }));

            await context.SaveChangesAsync(CancellationToken.None);
        }

        // Act
        using (var scope = fixture.Provider.Value.CreateScope())
        {
            var processor = scope.ServiceProvider.GetRequiredService<IQueueProcessor>();

            await processor.ProcessNextOutboxQueueMessage(CancellationToken.None);
        }

        // Assert

        using (var scope = fixture.Provider.Value.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);

            var updatedMessages = await context.OutboxMessages.ToListAsync(CancellationToken.None);

            Assert.Single(updatedMessages);

            Assert.Equal(1, updatedMessages[0].Retries);
        }
    }
}