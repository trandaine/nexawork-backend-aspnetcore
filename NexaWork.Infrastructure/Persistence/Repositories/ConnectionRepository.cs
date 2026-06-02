using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;
using NexaWork.Infrastructure.Persistence;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class ConnectionRepository : IConnectionRepository
{
    private readonly NexaWorkDbContext _context;

    public ConnectionRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public async Task<Connection?> GetByIdAsync(Guid connectionId, CancellationToken cancellationToken = default)
    {
        return await _context.Connections
            .Include(c => c.Customer)
            .Include(c => c.ConnectedCustomer)
            .FirstOrDefaultAsync(c => c.ConnectionId == connectionId, cancellationToken);
    }

    public async Task<Connection?> GetConnectionAsync(Guid customerId, Guid connectedCustomerId, CancellationToken cancellationToken = default)
    {
        return await _context.Connections
            .FirstOrDefaultAsync(c => 
                (c.CustomerId == customerId && c.ConnectedCustomerId == connectedCustomerId) ||
                (c.CustomerId == connectedCustomerId && c.ConnectedCustomerId == customerId), 
                cancellationToken);
    }

    public async Task AddAsync(Connection connection, CancellationToken cancellationToken = default)
    {
        await _context.Connections.AddAsync(connection, cancellationToken);
    }

    public void Update(Connection connection)
    {
        _context.Connections.Update(connection);
    }

    public void Remove(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<List<Connection>> GetPendingConnectionsAsync(Guid receiverCustomerId, CancellationToken cancellationToken = default)
    {
        return await _context.Connections
            .Include(c => c.Customer)
            .Where(c => c.ConnectedCustomerId == receiverCustomerId && c.Status == ConnectionStatus.Pending)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Connection>> GetConnectionsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        // A connection is valid if it's Accepted and the user is either the sender or the receiver
        return await _context.Connections
            .Include(c => c.Customer)
            .Include(c => c.ConnectedCustomer)
            .Where(c => (c.CustomerId == customerId || c.ConnectedCustomerId == customerId) && c.Status == ConnectionStatus.Accepted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
