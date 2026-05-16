using MediatR;

namespace NexaWork.Application.Features.Client.Comment.Commands.Create;

public record CreateCommentCommand(Guid PostId, Guid CustomerId, string Content) : IRequest<Guid>;