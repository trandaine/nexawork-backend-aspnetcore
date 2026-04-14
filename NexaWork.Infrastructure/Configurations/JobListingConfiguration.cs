using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Configurations;

public class JobListingConfiguration : IEntityTypeConfiguration<JobListing>
{
    public void Configure(EntityTypeBuilder<JobListing> builder)
    {
        builder.HasKey(j => j.JobListingId);

        builder.Property(j => j.Title).IsRequired().HasMaxLength(150);
        builder.Property(j => j.Location).HasMaxLength(150);
        builder.Property(j => j.SalaryRange).HasMaxLength(50);
        builder.Property(j => j.ContactEmail).HasMaxLength(100);
        
        // No MaxLength defaults to nvarchar(max) for long text
        builder.Property(j => j.Description).IsRequired(); 
        builder.Property(j => j.Requirements).IsRequired();

        builder.HasOne(j => j.Organization)
               .WithMany(o => o.JobListings)
               .HasForeignKey(j => j.OrganizationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}