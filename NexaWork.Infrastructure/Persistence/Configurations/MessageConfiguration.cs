using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.MessageId);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(m => m.IsRead)
            .HasDefaultValue(false);

        builder.Property(m => m.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        // Composite indexes for fast conversation querying
        builder.HasIndex(m => new { m.SenderCustomerId, m.ReceiverCustomerId, m.CreatedAt });
        builder.HasIndex(m => new { m.ReceiverCustomerId, m.SenderCustomerId, m.CreatedAt });

        // Global query filter for soft delete
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
