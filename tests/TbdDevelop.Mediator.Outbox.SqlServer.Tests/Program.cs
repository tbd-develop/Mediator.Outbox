using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TbdDevelop.Mediator.Outbox.Extensions;
using TbdDevelop.Mediator.Outbox.SqlServer.Context;
using TbdDevelop.Mediator.Outbox.SqlServer.Extensions;
using TbdDevelop.Mediator.Outbox.SqlServer.Models;
using Testcontainers.MsSql;

var msSqlContainer = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
    .Build();

await msSqlContainer.StartAsync();

var builder = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMediatorOutbox(builder =>
        {
            builder.UseSqlServerOutbox(msSqlContainer.GetConnectionString());
        });
    });

var app = builder.Build();

app.ConfigureSqlOutbox();

var factory = app.Services.GetRequiredService<IDbContextFactory<OutboxDbContext>>();

await using var context = factory.CreateDbContext();

var outboxMessage = new OutboxMessage
{
    Content = "Message",
    Type = "Type",
    DateAdded = DateTime.UtcNow,
    DateProcessed = null
};

await context.OutboxMessages.AddAsync(outboxMessage);

await context.SaveChangesAsync();

var outboxMessages = await context.OutboxMessages.ToListAsync();

Console.WriteLine($"Message Count: {outboxMessages.Count}");

await app.RunAsync();