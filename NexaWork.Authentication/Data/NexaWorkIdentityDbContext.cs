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

}
