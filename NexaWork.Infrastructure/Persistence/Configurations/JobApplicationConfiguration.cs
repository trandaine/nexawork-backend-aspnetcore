using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(ja => ja.JobApplicationId);

        builder.Property(ja => ja.ResumeUrl).IsRequired().HasMaxLength(255);
        builder.Property(ja => ja.CoverLetter).HasMaxLength(2000);

        builder.HasOne(ja => ja.JobListing)
               .WithMany(jl => jl.Applications)
               .HasForeignKey(ja => ja.JobListingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ja => ja.Customer)
               .WithMany(c => c.JobApplications)
               .HasForeignKey(ja => ja.CustomerId)
               .OnDelete(DeleteBehavior.NoAction); // Tránh cascade path conflict
    }
}
