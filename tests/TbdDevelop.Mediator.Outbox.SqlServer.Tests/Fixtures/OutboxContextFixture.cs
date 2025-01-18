using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Services;
using TbdDevelop.Mediator.Outbox.SqlServer.Context;
using TbdDevelop.Mediator.Outbox.SqlServer.Tests.Stubs;
using Testcontainers.MsSql;
using Xunit;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Tests.Fixtures;

public class OutboxContextFixture : IAsyncLifetime
{
    public Lazy<IServiceProvider> Provider =>
        new(() => _services.BuildServiceProvider());

    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly MsSqlContainer _msSqlContainer;

    public OutboxContextFixture()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithPassword("Password1234!")
            .Build();

        _services.AddPooledDbContextFactory<OutboxDbContext>(
            factory => factory.UseSqlServer(_msSqlContainer.GetConnectionString()));

        _services.AddTransient<IOutbox, SqlServerNotificationOutbox>();
        _services.AddTransient<IOutboxStorage, SqlServerOutboxStorage>();
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
        await _msSqlContainer.StartAsync();

        var factory = Provider.Value.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

        await using var context = await factory.CreateDbContextAsync();

        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}