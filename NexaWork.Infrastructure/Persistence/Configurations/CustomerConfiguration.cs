using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.CustomerId);
        
        builder.Property(c => c.IdentityUserId).IsRequired().HasMaxLength(450);
        builder.HasIndex(c => c.IdentityUserId).IsUnique();
        builder.Property(c => c.FirstName).HasMaxLength(50);
        builder.Property(c => c.LastName).HasMaxLength(50);
        builder.Property(c => c.Headline).HasMaxLength(150);
        builder.Property(c => c.Summary).HasMaxLength(1000);
        builder.Property(c => c.Location).HasMaxLength(100);
        builder.Property(c => c.ProfilePictureUrl).HasMaxLength(255);
        builder.Property(c => c.BackgroundPictureUrl).HasMaxLength(255);
        builder.Property(c => c.PhoneNumber).HasMaxLength(16);
    }
}
