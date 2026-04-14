using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.OrganizationId);

        builder.Property(o => o.Name).IsRequired().HasMaxLength(150);
        builder.Property(o => o.Industry).HasMaxLength(100);
        builder.Property(o => o.Location).HasMaxLength(150);
        builder.Property(o => o.Description).HasMaxLength(2000);
        builder.Property(o => o.WebsiteUrl).HasMaxLength(255);
        builder.Property(o => o.OrganizationLogoUrl).HasMaxLength(255);
    }
}
