using NexaWork.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexaWork.Application.Common.Interfaces.Repositories
{
    public interface IReactionRepository
    {
        void Add(Reaction reaction);
        void Remove(Reaction reaction);
        Task<Reaction?> GetByCustomerIdAndPostIdAsync(Guid customerId, Guid postId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid customerId, Guid postId, CancellationToken cancellationToken = default);
    }
}