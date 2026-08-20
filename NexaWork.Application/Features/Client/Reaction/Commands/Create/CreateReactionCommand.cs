using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Create;

public record CreateReactionCommand(Guid PostId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}