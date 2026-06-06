using MediatR;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Create;

public record CreateReactionCommand(Guid PostId) : IRequest;