using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Application.DTOs.Connections;

namespace NexaWork.Application.Features.Client.Connections.Queries.GetPendingConnections;

public class GetPendingConnectionsQueryHandler : IRequestHandler<GetPendingConnectionsQuery, List<ConnectionDto>>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingConnectionsQueryHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<ConnectionDto>> Handle(GetPendingConnectionsQuery request, CancellationToken cancellationToken)
    {
        var identityId = _currentUserService.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        var connections = await _connectionRepository.GetPendingConnectionsAsync(currentUser.CustomerId, cancellationToken);

        return connections.Select(c => new ConnectionDto
        {
            ConnectionId = c.ConnectionId,
            CustomerId = c.CustomerId,
            ConnectedCustomerId = c.ConnectedCustomerId,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            TargetUserId = c.Customer.CustomerId,
            TargetFirstName = c.Customer.FirstName ?? string.Empty,
            TargetLastName = c.Customer.LastName ?? string.Empty,
            TargetHeadline = c.Customer.Headline,
            TargetProfilePictureUrl = c.Customer.ProfilePictureUrl
        }).ToList();
    }
}
