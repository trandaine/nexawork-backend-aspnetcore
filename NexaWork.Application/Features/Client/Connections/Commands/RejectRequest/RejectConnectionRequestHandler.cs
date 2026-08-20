using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Connections.Commands.RejectRequest;

public class RejectConnectionRequestHandler : IRequestHandler<RejectConnectionRequestCommand>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public RejectConnectionRequestHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RejectConnectionRequestCommand request, CancellationToken cancellationToken)
    {
        var identityId = request.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        var connection = await _connectionRepository.GetByIdAsync(request.ConnectionId, cancellationToken);
        if (connection == null)
            throw new KeyNotFoundException("Connection request not found");

        if (connection.ConnectedCustomerId != currentUser.CustomerId)
            throw new InvalidOperationException("You are not authorized to reject this request");

        if (connection.Status != ConnectionStatus.Pending)
            throw new InvalidOperationException($"Cannot reject a request with status: {connection.Status}");

        // As per user requirement, we keep the record to prevent spam requests.
        connection.Status = ConnectionStatus.Rejected;
        _connectionRepository.Update(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
