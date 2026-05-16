using MediatR;
using NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

namespace NexaWork.Application.Features.Client.Comment.Queries.GetAllCommentsByPostId;

// A post can inclue no comments. So we set CommentQueryDTO can be nullable.
public record GetAllCommentsByPostIdQuery(Guid PostId): IRequest<List<CommentQueryDTO>>;