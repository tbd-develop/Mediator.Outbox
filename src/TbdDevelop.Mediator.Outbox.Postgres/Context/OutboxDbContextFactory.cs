using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TbdDevelop.Mediator.Outbox.Postgres.Context;

public class OutboxDbContextFactory : IDesignTimeDbContextFactory<OutboxDbContext>
{
    public OutboxDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OutboxDbContext>();

        // Use a dummy connection string for design-time - migrations don't need a real database
        optionsBuilder.UseNpgsql("Server=localhost;Database=DesignTimeOnly;Trusted_Connection=true;");

        return new OutboxDbContext(optionsBuilder.Options);
    }
}