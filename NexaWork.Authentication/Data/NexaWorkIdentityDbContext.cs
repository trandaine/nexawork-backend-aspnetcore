using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexaWork.Authentication.Data.IdentityEntities;

namespace NexaWork.Authentication.Data;

public class NexaWorkIdentityDbContext : IdentityDbContext<NexaWorkUser, NexaWorkRole, string>
{
    public NexaWorkIdentityDbContext(DbContextOptions<NexaWorkIdentityDbContext> options) : base(options)
    {
    }
    public DbSet<NexaWorkUser> NexaWorkUsers { get; set; }
    public DbSet<NexaWorkRole> NexaWorkRoles { get; set; }
    public DbSet<FidoStoredCredential> FidoStoredCredentials { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<NexaWorkUser>(entity =>
        {
            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.Preferred2faMethod).HasMaxLength(50);
        });

        builder.Entity<NexaWorkRole>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        builder.Entity<FidoStoredCredential>(entity =>
        {
            entity.HasOne(c => c.User)
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.UseOpenIddict();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Server= 100.81.167.42, 1433; Database=NexaWorkAuthenticationDatabase; User Id=sa; password=Dai@2018; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            // var connectionString = "Server= 100.125.57.47, 1433; Database=NexaWorkAuthenticationDatabase; User Id=sa; password=lohosum619@@; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
