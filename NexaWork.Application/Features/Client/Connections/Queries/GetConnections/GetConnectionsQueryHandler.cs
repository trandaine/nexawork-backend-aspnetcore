using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.DTOs.Connections;

namespace NexaWork.Application.Features.Client.Connections.Queries.GetConnections;

public class GetConnectionsQueryHandler : IRequestHandler<GetConnectionsQuery, List<ConnectionDto>>
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetConnectionsQueryHandler(
        IConnectionRepository connectionRepository,
        ICustomerRepository customerRepository)
    {
        _connectionRepository = connectionRepository;
        _customerRepository = customerRepository;
    }

    public async Task<List<ConnectionDto>> Handle(GetConnectionsQuery request, CancellationToken cancellationToken)
    {
        var identityId = request.UserId;
        var currentUser = await _customerRepository.GetByIdentityIdToEditAsync(identityId, cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not found");

        var connections = await _connectionRepository.GetConnectionsAsync(currentUser.CustomerId, cancellationToken);

        return connections.Select(c => 
        {
            var targetUser = c.CustomerId == currentUser.CustomerId ? c.ConnectedCustomer : c.Customer;
            return new ConnectionDto
            {
                ConnectionId = c.ConnectionId,
                CustomerId = c.CustomerId,
                ConnectedCustomerId = c.ConnectedCustomerId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                TargetUserId = targetUser.CustomerId,
                TargetFirstName = targetUser.FirstName ?? string.Empty,
                TargetLastName = targetUser.LastName ?? string.Empty,
                TargetHeadline = targetUser.Headline,
                TargetProfilePictureUrl = targetUser.ProfilePictureUrl
            };
        }).ToList();
    }
}
