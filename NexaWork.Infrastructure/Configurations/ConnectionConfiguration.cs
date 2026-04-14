using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Configurations;

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.HasKey(c => c.ConnectionId);

        // Crucial: Restrict cascade deletes to avoid multiple cascade paths in SQL Server
        builder.HasOne(c => c.Customer)
               .WithMany(u => u.SentConnections)
               .HasForeignKey(c => c.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ConnectedCustomer)
               .WithMany(u => u.ReceivedConnections)
               .HasForeignKey(c => c.ConnectedCustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
