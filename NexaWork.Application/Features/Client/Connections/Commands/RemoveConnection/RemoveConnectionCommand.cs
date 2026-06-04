using MediatR;
using System;

namespace NexaWork.Application.Features.Client.Connections.Commands.RemoveConnection;

public record RemoveConnectionCommand(Guid TargetCustomerId) : IRequest;
