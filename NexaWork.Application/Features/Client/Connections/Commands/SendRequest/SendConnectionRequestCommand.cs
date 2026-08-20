using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.SendRequest;

public record SendConnectionRequestCommand(Guid TargetCustomerId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
