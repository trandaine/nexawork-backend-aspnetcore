using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.RejectRequest;

public record RejectConnectionRequestCommand(Guid ConnectionId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
