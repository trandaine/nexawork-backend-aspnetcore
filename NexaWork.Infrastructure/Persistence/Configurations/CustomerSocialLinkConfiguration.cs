using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class CustomerSocialLinkConfiguration : IEntityTypeConfiguration<CustomerSocialLink>
{
    public void Configure(EntityTypeBuilder<CustomerSocialLink> builder)
    {
        builder.ToTable("CustomerSocialLinks");
        builder.HasKey(ca => ca.CustomerSocialLinkId);
        
        builder.Property(ca => ca.FaceBookUrl).HasMaxLength(200);
        builder.Property(ca => ca.LinkedInUrl).HasMaxLength(200);
        builder.Property(ca => ca.XUrl).HasMaxLength(200);
        builder.Property(ca => ca.InstagramUrl).HasMaxLength(200);
    }
}