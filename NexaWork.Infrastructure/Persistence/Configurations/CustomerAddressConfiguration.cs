using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Configurations;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");
        builder.HasKey(ca => ca.CustomerAddressId);
        
        builder.Property(ca => ca.City).HasMaxLength(100);
        builder.Property(ca => ca.PostalCode).HasMaxLength(20);
        builder.Property(ca => ca.Country).HasMaxLength(100);
        builder.Property(ca => ca.TaxId).HasMaxLength(50);
    }
}