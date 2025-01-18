using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Outbox.SqlServer.Models;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Context;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options)
    : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<DeadLetterMessage> DeadLetterMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfigurationsFromAssembly(typeof(OutboxDbContext).Assembly);
    }
}