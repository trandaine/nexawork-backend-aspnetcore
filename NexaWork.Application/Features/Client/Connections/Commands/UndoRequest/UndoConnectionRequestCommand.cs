using MediatR;
using System;

namespace NexaWork.Application.Features.Client.Connections.Commands.UndoRequest;

public record UndoConnectionRequestCommand(Guid TargetCustomerId) : IRequest;
