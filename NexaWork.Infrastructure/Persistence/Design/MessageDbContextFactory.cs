using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NexaWork.Domain.Constants;
using NexaWork.Infrastructure.Persistence;

namespace NexaWork.Infrastructure.Persistence.Design;

public class MessageDbContextFactory : IDesignTimeDbContextFactory<MessageDbContext>
{
    public MessageDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MessageDbContext>();
        optionsBuilder.UseSqlServer(ConnectionStringConstants.MessageConnectionString);

        return new MessageDbContext(optionsBuilder.Options);
    }
}
