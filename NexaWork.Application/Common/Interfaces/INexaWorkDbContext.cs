using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces;

public interface INexaWorkDbContext
{
    DbSet<Customer> Customers { get;  }
    DbSet<Organization> Organizations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
