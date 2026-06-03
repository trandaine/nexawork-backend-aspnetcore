using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface IConnectionRepository
{
    /// <summary>
    /// Get connection by its unique identifier
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Connection?> GetByIdAsync(Guid connectionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get connection status of customer who owns the request and customer who received the request
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="connectedCustomerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Connection?> GetConnectionAsync(Guid customerId, Guid connectedCustomerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add new request record
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(Connection connection, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update existed connection
    /// </summary>
    /// <param name="connection"></param>
    void Update(Connection connection);
    void Remove(Connection connection);
    
    /// <summary>
    /// Get Pending customer connection requests for a specific customer, ordered by created date descending
    /// </summary>
    /// <param name="receiverCustomerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Connection>> GetPendingConnectionsAsync(Guid receiverCustomerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all customer connections for a specific customer who have been accepted, ordered by created date descending
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Connection>> GetConnectionsAsync(Guid customerId, CancellationToken cancellationToken = default);
}
