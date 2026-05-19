using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Domain.Constants;
using NexaWork.Domain.Entities;
using NexaWork.Infrastructure.Persistence.Configurations;

namespace NexaWork.Infrastructure.Persistence;

public class NexaWorkDbContext : DbContext, INexaWorkDbContext
{
    public NexaWorkDbContext()
    {
    }

    public NexaWorkDbContext(DbContextOptions<NexaWorkDbContext> options) : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = ConnectionStringConstants.ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new ConnectionConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerSkillConfiguration());
        modelBuilder.ApplyConfiguration(new EducationConfiguration());
        modelBuilder.ApplyConfiguration(new ExperienceConfiguration());
        modelBuilder.ApplyConfiguration(new JobApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new JobListingConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new PostConfiguration());
        modelBuilder.ApplyConfiguration(new ReactionConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerSocialLinkConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerAddressConfiguration());
        
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Hook point for:
        // - audit fields
        // - domain events
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Skill> Skills => Set<Skill>();
    // public DbSet<Comment> Comments { get; set; }
    public DbSet<Connection> Connections { get; set; }

    public DbSet<Customer> Customers => Set<Customer>();

    // public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerSkill> CustomerSkills { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<JobListing> JobListings { get; set; }

    public DbSet<Organization> Organizations => Set<Organization>();

    // public DbSet<Organization> Organizations { get; set; }
    public DbSet<Post> Posts => Set<Post>();

    // public DbSet<Post> Posts { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    public DbSet<CustomerSocialLink> CustomerSocialLinks => Set<CustomerSocialLink>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    // public DbSet<Skill> Skills { get; set; }
}