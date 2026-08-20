using NexaWork.Application.Common.Interfaces;
using MediatR;
using System;

namespace NexaWork.Application.Features.Client.Connections.Commands.RemoveConnection;

public record RemoveConnectionCommand(Guid TargetCustomerId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
