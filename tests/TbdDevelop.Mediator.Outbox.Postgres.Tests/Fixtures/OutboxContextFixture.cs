using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Postgres.Context;
using TbdDevelop.Mediator.Outbox.Postgres.Tests.Stubs;
using TbdDevelop.Mediator.Outbox.Services;
using Testcontainers.PostgreSql;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.Postgres.Tests.Fixtures;

public class OutboxContextFixture : IAsyncLifetime
{
    public Lazy<IServiceProvider> Provider =>
        new(() => _services.BuildServiceProvider());

    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly PostgreSqlContainer _sqlContainer;

    public OutboxContextFixture()
    {
        _sqlContainer = new PostgreSqlBuilder("postgres:17-alpine")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithPassword("Password1234!")
            .WithDockerEndpoint("npipe://./pipe/docker_engine")
            .WithCleanUp(true)
            .Build();

        _services.AddPooledDbContextFactory<OutboxDbContext>(factory =>
            factory.UseNpgsql(_sqlContainer.GetConnectionString()));

        _services.AddTransient<IOutbox, PostgresNotificationOutbox>();
        _services.AddTransient<IOutboxStorage, PostgresOutboxStorage>();
        _services.AddTransient<IOutboxProcessingPublisher, OutboxProcessingPublisherStub>();
        _services.AddTransient<INotificationPublisher, OutboxPublisher>();
        _services.AddTransient<IQueueProcessor, OutboxMessageProcessor>();

        _services.AddLogging(builder =>
        {
            builder.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger());
        });
    }

    public TestOutputRedirect RedirectOutput(ITestOutputHelper output) => new(output);

    public async ValueTask InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        var factory = Provider.Value.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

        await using var context = await factory.CreateDbContextAsync();

        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }
}