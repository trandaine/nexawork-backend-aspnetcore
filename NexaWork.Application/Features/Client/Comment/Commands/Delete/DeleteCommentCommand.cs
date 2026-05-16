using MediatR;

namespace NexaWork.Application.Features.Client.Comment.Commands.Delete;

public record DeleteCommentCommand(Guid CommentId) : IRequest;