using Microsoft.EntityFrameworkCore;
using TbdDevelop.Mediator.Outbox.Postgres.Models;

namespace TbdDevelop.Mediator.Outbox.Postgres.Context;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options)
    : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<DeadLetterMessage> DeadLetterMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("outbox")
            .ApplyConfigurationsFromAssembly(typeof(OutboxDbContext).Assembly);
    }
}