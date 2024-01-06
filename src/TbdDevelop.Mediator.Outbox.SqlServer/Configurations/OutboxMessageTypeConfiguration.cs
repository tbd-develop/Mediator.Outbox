using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TbdDevelop.Mediator.Outbox.SqlServer.Models;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Configurations;

public class OutboxMessageTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("NotificationOutbox");
        
        builder.HasKey(k => k.Id);
        
        builder.Property(p => p.DateAdded)
            .HasDefaultValueSql("getutcdate()");
    }
}