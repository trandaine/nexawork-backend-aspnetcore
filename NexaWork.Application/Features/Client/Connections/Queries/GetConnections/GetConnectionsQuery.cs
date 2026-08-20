using NexaWork.Application.Common.Interfaces;
using MediatR;
using NexaWork.Application.DTOs.Connections;

namespace NexaWork.Application.Features.Client.Connections.Queries.GetConnections;

public record GetConnectionsQuery() : IRequest<List<ConnectionDto>>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
