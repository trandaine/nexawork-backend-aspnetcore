using MediatR;

namespace NexaWork.Application.Features.Client.Connections.Commands.BlockConnection;

public record BlockConnectionCommand(Guid TargetCustomerId) : IRequest;
