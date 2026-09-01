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
        if (connection == null)
            throw new InvalidOperationException("Cannot block a user who is not an accepted connection.");

        if (connection.Status != ConnectionStatus.Accepted)
            throw new InvalidOperationException($"Can only block users with an active Accepted connection. Current status: {connection.Status}");

        // Preserve previous status to restore on unblock
        connection.StatusBeforeBlock = connection.Status;

        // Ensure the blocker is the owner of the block record
        connection.CustomerId = currentUser.CustomerId;
        connection.ConnectedCustomerId = request.TargetCustomerId;
        connection.Status = ConnectionStatus.Blocked;

        _connectionRepository.Update(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
