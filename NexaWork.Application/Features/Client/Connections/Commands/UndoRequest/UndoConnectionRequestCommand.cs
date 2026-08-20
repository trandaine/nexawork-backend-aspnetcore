using NexaWork.Application.Common.Interfaces;
using MediatR;
using System;

namespace NexaWork.Application.Features.Client.Connections.Commands.UndoRequest;

public record UndoConnectionRequestCommand(Guid TargetCustomerId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
