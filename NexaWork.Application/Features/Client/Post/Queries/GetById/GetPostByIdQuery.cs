using MediatR;

namespace NexaWork.Application.Features.Client.Post.Queries.GetById;

public record GetPostByIdQuery(Guid PostId) : IRequest<PostQueryDTO?>;