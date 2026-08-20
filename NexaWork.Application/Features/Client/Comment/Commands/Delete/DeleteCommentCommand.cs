using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Comment.Commands.Delete;

public record DeleteCommentCommand(Guid CommentId) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}