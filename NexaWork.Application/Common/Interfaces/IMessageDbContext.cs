using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces;

public interface IMessageDbContext
{
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
