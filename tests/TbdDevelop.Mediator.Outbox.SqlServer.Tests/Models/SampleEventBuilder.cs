using System.Text.Json;
using Mediator;
using TbdDevelop.Mediator.Outbox.SqlServer.Models;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Tests.Models;

public abstract class SampleEventBuilder<T>
    where T : INotification, new()
{
    public static OutboxMessage AsOutboxMessage(Action<T> configure, int retries = 0)
    {
        var result = new T();

        configure(result);

        return new OutboxMessage
        {
            Type = typeof(T).AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(result),
            DateAdded = DateTime.UtcNow,
            Retries = retries
        };
    }
}