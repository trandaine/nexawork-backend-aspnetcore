using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasKey(r => r.ReactionId);

        builder.HasOne(r => r.Customer)
               .WithMany(c => c.Reactions)
               .HasForeignKey(r => r.CustomerId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.Post)
               .WithMany(p => p.Reactions)
               .HasForeignKey(r => r.PostId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.Comment)
               .WithMany(c => c.Reactions)
               .HasForeignKey(r => r.CommentId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}