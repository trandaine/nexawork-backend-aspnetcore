using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {

        builder.HasKey(e => e.EducationId);
        
        builder.Property(e => e.SchoolName).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Degree).HasMaxLength(100);
        builder.Property(e => e.FieldOfStudy).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(1000);

        builder.HasOne(e => e.Customer)
               .WithMany(c => c.Educations)
               .HasForeignKey(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
