using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.SqlServer.Context;
using TbdDevelop.Mediator.Outbox.SqlServer.Tests.Fixtures;
using TbdDevelop.Mediator.Outbox.SqlServer.Tests.Models;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Tests;

public class WhenNotificationIsPublished_AndRetryLimitIsExceeded(
    OutboxContextFixture fixture,
    ITestOutputHelper outputHelper)
    : IClassFixture<OutboxContextFixture>
{
    private const int DefaultRetryLimit = 3;
    
    [Fact]
    public async Task message_is_moved_to_dead_letter_queue()
    {
        await using (fixture.RedirectOutput(outputHelper)) ;

        // Arrange 

        using (var scope = fixture.Provider.Value.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);

            context.OutboxMessages.Add(
                SampleEventBuilder<SampleErrorEventType>.AsOutboxMessage(
                    msg => { msg.Content = "Pending Message"; }, retries: DefaultRetryLimit));

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

            var deadLetterMessages = await context.DeadLetterMessages.ToListAsync(CancellationToken.None);

            Assert.Single(deadLetterMessages);
        }
    }
}