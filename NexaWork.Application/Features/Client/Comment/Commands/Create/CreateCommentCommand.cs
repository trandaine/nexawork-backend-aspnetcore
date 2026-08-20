using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Comment.Commands.Create;

public record CreateCommentCommand(
    Guid PostId,
    string Content
) : IRequest<Guid>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}