using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Interfaces;
using NexaWork.Domain.Constants;
using NexaWork.Domain.Entities;
using NexaWork.Infrastructure.Configurations;

namespace NexaWork.Infrastructure;

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
    }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerSkill> CustomerSkills { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<JobListing> JobListings { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Skill> Skills { get; set; }
}
