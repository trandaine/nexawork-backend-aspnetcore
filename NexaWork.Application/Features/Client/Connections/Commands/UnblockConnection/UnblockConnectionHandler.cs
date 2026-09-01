using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexaWork.Application.Features.Client.Connections.Commands.UnblockConnection;

public class UnblockConnectionHandler : IRequestHandler<UnblockConnectionCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public UnblockConnectionHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UnblockConnectionCommand request, CancellationToken cancellationToken)
    {
        var identityId = request.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        if (currentUser.CustomerId == request.TargetCustomerId)
            throw new InvalidOperationException("Cannot unblock yourself");

        var targetUser = await _customerRepository.GetCustomerByIdAsync(request.TargetCustomerId, cancellationToken);
        if (targetUser == null)
            throw new KeyNotFoundException("Target user not found");

        var connection = await _connectionRepository.GetConnectionAsync(currentUser.CustomerId, request.TargetCustomerId, cancellationToken);
        if (connection == null)
            throw new KeyNotFoundException("Connection not found");

        if (connection.Status != ConnectionStatus.Blocked)
            throw new InvalidOperationException("This connection is not currently blocked.");

        if (connection.CustomerId != currentUser.CustomerId)
            throw new UnauthorizedAccessException("Only the user who initiated the block can unblock.");

        // Restore to previous status (default to Accepted if not tracked)
        connection.Status = connection.StatusBeforeBlock ?? ConnectionStatus.Accepted;
        connection.StatusBeforeBlock = null;

        _connectionRepository.Update(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
