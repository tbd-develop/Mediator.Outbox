using System.Text.Json;
using Mediator;
using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.SqlServer.Context;
using TbdDevelop.Mediator.Outbox.SqlServer.Models;

namespace TbdDevelop.Mediator.Outbox.SqlServer;

public class SqlServerNotificationOutbox(IDbContextFactory<OutboxDbContext> factory) : IOutbox
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask Add<TNotification>(TNotification notification, CancellationToken cancellationToken = new())
        where TNotification : INotification
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);

        await context.OutboxMessages.AddAsync(new OutboxMessage
        {
            Type = notification.GetType().AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(notification, SerializerOptions),
            DateAdded = DateTime.UtcNow
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}