using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Postgres.Context;
using TbdDevelop.Mediator.Outbox.Postgres.Models;

namespace TbdDevelop.Mediator.Outbox.Postgres;

public class PostgresOutboxStorage(IDbContextFactory<OutboxDbContext> factory) : IOutboxStorage
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<IOutboxMessage?> RetrieveNextMessage(CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);

        var message = await context.OutboxMessages
            .Where(m => m.DateProcessed == null)
            .OrderBy(m => m.Retries)
            .ThenBy(m => m.DateAdded)
            .FirstOrDefaultAsync(cancellationToken);

        return message is null ? null : BuildOutboxMessage(message);
    }

    private IOutboxMessage? BuildOutboxMessage(OutboxMessage message)
    {
        var type = Type.GetType(message.Type);

        if (type is null)
        {
            return null;
        }

        var content = JsonSerializer.Deserialize(message.Content, type, SerializerOptions);

        return (IOutboxMessage)Activator.CreateInstance(
            typeof(PostgresOutboxMessage<>).MakeGenericType(type),
            message.Id, message.Retries, message.DateAdded, content)!;
    }

    public async Task Commit(IOutboxMessage message, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);

        var outboxMessage =
            await context.OutboxMessages.SingleOrDefaultAsync(m => m.Id == (int)message.Id, cancellationToken);

        if (outboxMessage is null)
        {
            return;
        }

        outboxMessage.DateProcessed = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task IncreaseRetryCount(IOutboxMessage message, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);

        var outboxMessage =
            await context.OutboxMessages
                .SingleOrDefaultAsync(m => m.Id == (int)message.Id, cancellationToken);

        if (outboxMessage is null)
        {
            return;
        }

        outboxMessage.Retries += 1;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MoveToDeadLetterQueue(IOutboxMessage message, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken);

        var outboxMessage =
            await context.OutboxMessages.SingleOrDefaultAsync(m => m.Id == (int)message.Id, cancellationToken);

        if (outboxMessage is null)
        {
            return;
        }

        context.DeadLetterMessages.Add(new DeadLetterMessage
        {
            Type = outboxMessage.Type,
            Content = outboxMessage.Content,
            DateAdded = outboxMessage.DateAdded
        });

        context.OutboxMessages.Remove(outboxMessage);

        await context.SaveChangesAsync(cancellationToken);
    }
}