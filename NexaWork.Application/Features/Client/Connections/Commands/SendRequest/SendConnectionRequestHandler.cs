using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Connections.Commands.SendRequest;

public class SendConnectionRequestHandler : IRequestHandler<SendConnectionRequestCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public SendConnectionRequestHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles sending a connection request to a target customer by UserId.
    /// </summary>
    /// <param name="request">The connection request command containing target customer details and sender identity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the sender identity cannot be resolved to a customer.</exception>
    /// <exception cref="InvalidOperationException">Thrown when attempting to connect to oneself, when a connection already exists, or when the connection is blocked.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the target customer does not exist.</exception>
    public async Task Handle(SendConnectionRequestCommand request, CancellationToken cancellationToken)
    {
        var identityId = request.UserId;
        var sender = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (sender == null)
            throw new UnauthorizedAccessException("User not found");

        if (sender.CustomerId == request.TargetCustomerId)
            throw new InvalidOperationException("Cannot connect with yourself");

        var target = await _customerRepository.GetCustomerByIdAsync(request.TargetCustomerId, cancellationToken);
        if (target == null)
            throw new KeyNotFoundException("Target user not found");

        var existingConnection = await _connectionRepository.GetConnectionAsync(sender.CustomerId, request.TargetCustomerId, cancellationToken);
        
        if (existingConnection != null)
        {
            if (existingConnection.Status == ConnectionStatus.Blocked)
                throw new InvalidOperationException("Cannot send connection request to this user.");
            
            throw new InvalidOperationException($"A connection already exists with status: {existingConnection.Status}");
        }

        var connection = new Connection
        {
            ConnectionId = Guid.NewGuid(),
            CustomerId = sender.CustomerId,
            ConnectedCustomerId = request.TargetCustomerId,
            Status = ConnectionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _connectionRepository.AddAsync(connection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
