using MediatR;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Delete;

public record DeleteReactionCommand(Guid PostId) : IRequest;