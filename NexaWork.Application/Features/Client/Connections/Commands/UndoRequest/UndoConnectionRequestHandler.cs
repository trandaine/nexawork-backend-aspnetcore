using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexaWork.Application.Features.Client.Connections.Commands.UndoRequest;

public class UndoConnectionRequestHandler : IRequestHandler<UndoConnectionRequestCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INexaWorkDbContext _unitOfWork;

    public UndoConnectionRequestHandler(
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

    public async Task Handle(UndoConnectionRequestCommand request, CancellationToken cancellationToken)
    {
        var identityId = _currentUserService.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        var connection = await _connectionRepository.GetConnectionAsync(currentUser.CustomerId, request.TargetCustomerId, cancellationToken);
        if (connection == null)
            throw new KeyNotFoundException("Connection request not found");

        if (connection.CustomerId != currentUser.CustomerId)
            throw new InvalidOperationException("You are not authorized to undo this request as you are not the sender.");

        if (connection.Status != ConnectionStatus.Pending)
            throw new InvalidOperationException($"Cannot undo a request with status: {connection.Status}");

        _connectionRepository.Remove(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
