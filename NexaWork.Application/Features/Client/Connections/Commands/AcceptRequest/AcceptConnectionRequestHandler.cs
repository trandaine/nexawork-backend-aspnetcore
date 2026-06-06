using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Connections.Commands.AcceptRequest;

public class AcceptConnectionRequestHandler : IRequestHandler<AcceptConnectionRequestCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INexaWorkDbContext _unitOfWork;

    public AcceptConnectionRequestHandler(
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

    public async Task Handle(AcceptConnectionRequestCommand request, CancellationToken cancellationToken)
    {
        var identityId = _currentUserService.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        var connection = await _connectionRepository.GetByIdAsync(request.ConnectionId, cancellationToken);
        if (connection == null)
            throw new KeyNotFoundException("Connection request not found");

        if (connection.ConnectedCustomerId != currentUser.CustomerId)
            throw new InvalidOperationException("You are not authorized to accept this request");

        if (connection.Status != ConnectionStatus.Pending)
            throw new InvalidOperationException($"Cannot accept a request with status: {connection.Status}");

        connection.Status = ConnectionStatus.Accepted;
        _connectionRepository.Update(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
