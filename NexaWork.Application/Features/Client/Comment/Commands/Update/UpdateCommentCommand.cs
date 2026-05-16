using MediatR;

namespace NexaWork.Application.Features.Client.Comment.Commands.Update;

public record UpdateCommentCommand(Guid CommentId, string Content) : IRequest;