using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace NexaWork.Infrastructure.Persistence.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly NexaWorkDbContext _context;

        public ReactionRepository(NexaWorkDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Add new reaction
        /// </summary>
        /// <param name="reaction"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(Reaction reaction)
        {
            _context.Reactions.Add(reaction);
        }

        /// <summary>
        /// Remove a reaction
        /// </summary>
        /// <param name="reaction"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(Reaction reaction)
        {
            _context.Reactions.Remove(reaction);
        }

        public async Task<Reaction?> GetByCustomerIdAndPostIdAsync(Guid customerId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Reactions
                .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.PostId == postId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid customerId, Guid postId, CancellationToken cancellationToken = default)
        {
            return await _context.Reactions
                .AnyAsync(r => r.CustomerId == customerId && r.PostId == postId, cancellationToken);
        }
    }
}