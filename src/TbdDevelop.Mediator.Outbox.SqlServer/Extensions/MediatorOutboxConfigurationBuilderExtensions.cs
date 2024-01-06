using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Contracts;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;
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
        });

        return builder;
    }
}