using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Domain.Entities;
using NexaWork.Infrastructure.Persistence.Configurations;

namespace NexaWork.Infrastructure.Persistence;

public class MessageDbContext : DbContext, IMessageDbContext
{
    public MessageDbContext()
    {
    }

    public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Message> Messages => Set<Message>();
}
