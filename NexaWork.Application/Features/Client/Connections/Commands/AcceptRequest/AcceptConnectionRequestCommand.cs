using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.AcceptRequest;

public record AcceptConnectionRequestCommand(Guid ConnectionId) : IRequest;
