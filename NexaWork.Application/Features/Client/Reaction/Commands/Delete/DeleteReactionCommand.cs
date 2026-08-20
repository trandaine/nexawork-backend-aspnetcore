using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Delete;

public record DeleteReactionCommand(Guid PostId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}