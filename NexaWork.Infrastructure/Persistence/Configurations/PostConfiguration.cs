using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.PostId);
        
        builder.Property(p => p.Content).IsRequired(); // Tương đương nvarchar(max)
        builder.Property(p => p.MediaUrl).HasMaxLength(255);

        builder.HasOne(p => p.Customer)
               .WithMany(c => c.Posts)
               .HasForeignKey(p => p.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
