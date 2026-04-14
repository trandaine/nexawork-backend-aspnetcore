using System;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.CommentId);
        
        builder.Property(c => c.Content).IsRequired().HasMaxLength(1000);

        builder.HasOne(c => c.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(c => c.PostId)
               .OnDelete(DeleteBehavior.NoAction); // Tránh multiple cascade paths trong SQL Server

        builder.HasOne(c => c.Customer)
               .WithMany(u => u.Comments)
               .HasForeignKey(c => c.CustomerId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
