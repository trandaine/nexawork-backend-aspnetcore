using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexaWork.Application.Features.Client.Connections.Commands.RemoveConnection;

public class RemoveConnectionCommandHandler : IRequestHandler<RemoveConnectionCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INexaWorkDbContext _unitOfWork;

    public RemoveConnectionCommandHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        INexaWorkDbContext unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveConnectionCommand request, CancellationToken cancellationToken)
    {
        var identityId = _currentUserService.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        var connection = await _connectionRepository.GetConnectionAsync(currentUser.CustomerId, request.TargetCustomerId, cancellationToken);
        if (connection == null)
            throw new KeyNotFoundException("Connection not found");

        if (connection.Status != ConnectionStatus.Accepted)
            throw new InvalidOperationException($"Cannot remove a connection with status: {connection.Status}");

        _connectionRepository.Remove(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
