using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.RejectRequest;

public record RejectConnectionRequestCommand(Guid ConnectionId) : IRequest;
