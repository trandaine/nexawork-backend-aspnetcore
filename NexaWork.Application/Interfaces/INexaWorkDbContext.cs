using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Interfaces;

public interface INexaWorkDbContext
{
    DbSet<Customer> Customers { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
