using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Constants;
using NexaWork.Domain.IdentityEntites;

namespace NexaWork.Infrastructure;

public class NexaWorkDbIdentityContext : IdentityDbContext
{
    public NexaWorkDbIdentityContext(DbContextOptions<NexaWorkDbIdentityContext> options) : base(options)
    {

    }

    public DbSet<NexaWorkUser> NexaWorkUsers { get; set; }
    public DbSet<NexaWorkRole> NexaWorkRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<NexaWorkUser>(entity =>
        {
            entity.Property(e => e.Avatar).HasMaxLength(500);
        });

        builder.Entity<NexaWorkRole>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
        });

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // var connectionString = "Server= localhost, 1433; Database=NexaWorkIdentityDatabase; User Id=sa; password=Dai@2018; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            var connectionString = ConnectionStringConstants.IdentityConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
