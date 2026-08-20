using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.BlockConnection;

public record BlockConnectionCommand(Guid TargetCustomerId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
