using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexaWork.AuthServer.Data.IdentityEntities;

namespace NexaWork.AuthServer.Data;

public class NexaWorkIdentityDbContext : IdentityDbContext<NexaWorkUser, NexaWorkRole, string>
{
    public NexaWorkIdentityDbContext(DbContextOptions<NexaWorkIdentityDbContext> options) : base(options)
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
        builder.UseOpenIddict();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // var connectionString = "Server= localhost, 1433; Database=NexaWorkIdentityDatabase; User Id=sa; password=Dai@2018; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            var connectionString = "Server= localhost, 1433; Database=NexaWorkAuthenticationDatabase; User Id=sa; password=lohosum619@@; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
