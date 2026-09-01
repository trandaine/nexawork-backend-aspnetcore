using MediatR;
using NexaWork.Application.Common.Interfaces;

namespace NexaWork.Application.Features.Client.Connections.Commands.UnblockConnection;

public record UnblockConnectionCommand(Guid TargetCustomerId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
