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
