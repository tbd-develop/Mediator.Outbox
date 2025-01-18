using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.SqlServer.Context;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Extensions;

public static class MediatorOutboxConfigurationBuilderExtensions
{
    public static MediatorOutboxConfigurationBuilder UseSqlServerOutbox(this MediatorOutboxConfigurationBuilder builder,
        string? connectionString)
    {
        builder.Register(services =>
        {
            services.AddPooledDbContextFactory<OutboxDbContext>(configure =>
            {
                configure
                    .UseSqlServer(connectionString);
            });

            services.AddTransient<IOutbox, SqlServerNotificationOutbox>();
            services.AddTransient<IOutboxStorage, SqlServerOutboxStorage>();
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