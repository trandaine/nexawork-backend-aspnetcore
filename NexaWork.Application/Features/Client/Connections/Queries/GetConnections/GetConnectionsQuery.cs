using MediatR;
using NexaWork.Application.DTOs.Connections;

namespace NexaWork.Application.Features.Client.Connections.Queries.GetConnections;

public record GetConnectionsQuery() : IRequest<List<ConnectionDto>>;
