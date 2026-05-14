using MediatR;

namespace NexaWork.Application.Features.Client.Post.Queries.GetAll;

public record GetAllPostsQuery() : IRequest<List<PostQueryDTO>>;
