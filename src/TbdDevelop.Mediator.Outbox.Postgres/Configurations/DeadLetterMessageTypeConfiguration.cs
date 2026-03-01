using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TbdDevelop.Mediator.Outbox.Postgres.Models;

namespace TbdDevelop.Mediator.Outbox.Postgres.Configurations;

public class DeadLetterMessageTypeConfiguration : IEntityTypeConfiguration<DeadLetterMessage>
{
    public void Configure(EntityTypeBuilder<DeadLetterMessage> builder)
    {
        builder.ToTable("NotificationOutbox.DeadLetterQueue");

        builder.HasKey(k => k.Id);

        builder.Property(p => p.DateAdded)
            .HasDefaultValueSql("TIMEZONE('utc', NOW())");
    }
}