using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.SendRequest;

public record SendConnectionRequestCommand(Guid TargetCustomerId) : IRequest;
