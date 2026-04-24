using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class CustomerSkillConfiguration : IEntityTypeConfiguration<CustomerSkill>
{
    public void Configure(EntityTypeBuilder<CustomerSkill> builder)
    {
        builder.HasKey(cs => cs.CustomerSkillId);

        builder.HasOne(cs => cs.Customer)
               .WithMany(c => c.CustomerSkills)
               .HasForeignKey(cs => cs.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cs => cs.Skill)
               .WithMany(s => s.CustomerSkills)
               .HasForeignKey(cs => cs.SkillId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
