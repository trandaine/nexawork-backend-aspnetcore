using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Connections.Commands.BlockConnection;

public class BlockConnectionRequestHandler : IRequestHandler<BlockConnectionCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public BlockConnectionRequestHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BlockConnectionCommand request, CancellationToken cancellationToken)
    {
        var identityId = request.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        if (currentUser.CustomerId == request.TargetCustomerId)
            throw new InvalidOperationException("Cannot block yourself");

        var targetUser = await _customerRepository.GetCustomerByIdAsync(request.TargetCustomerId, cancellationToken);
        if (targetUser == null)
            throw new KeyNotFoundException("Target user not found");

        var connection = await _connectionRepository.GetConnectionAsync(currentUser.CustomerId, request.TargetCustomerId, cancellationToken);
        
        if (connection != null)
        {
            // Update the existing connection to Blocked
            connection.Status = ConnectionStatus.Blocked;
            _connectionRepository.Update(connection);
        }
        else
        {
            // Create a new blocked connection record
            var newConnection = new Connection
            {
                ConnectionId = Guid.NewGuid(),
                CustomerId = currentUser.CustomerId,
                ConnectedCustomerId = request.TargetCustomerId,
                Status = ConnectionStatus.Blocked,
                CreatedAt = DateTime.UtcNow
            };
            await _connectionRepository.AddAsync(newConnection, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
