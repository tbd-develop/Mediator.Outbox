using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Postgres.Context;

namespace TbdDevelop.Mediator.Outbox.Postgres.Extensions;

public static class MediatorOutboxConfigurationBuilderExtensions
{
    public static MediatorOutboxConfigurationBuilder UseNpgSqlOutbox(this MediatorOutboxConfigurationBuilder builder,
        string? connectionString)
    {
        builder.Register(services =>
        {
            services.AddPooledDbContextFactory<OutboxDbContext>(configure =>
            {
                configure
                    .UseNpgsql(connectionString);
            });

            services.AddTransient<IOutbox, PostgresNotificationOutbox>();
            services.AddTransient<IOutboxStorage, PostgresOutboxStorage>();
        });

        return builder;
    }

    public static IHost ConfigureSqlOutbox(this IHost host)
    {
        var factory = host.Services.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

        using var context = factory.CreateDbContext();

        context.Database.Migrate();

        return host;
    }
}