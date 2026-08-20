using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.AcceptRequest;

public record AcceptConnectionRequestCommand(Guid ConnectionId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
