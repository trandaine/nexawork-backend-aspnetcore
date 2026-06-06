using MediatR;

namespace NexaWork.Application.Features.Client.Post.Queries.GetById;

/// <summary>
/// Get posts for other customer 
/// </summary>
/// <param name="CustomerId">Specify customer who get</param>
public record GetPostByIdQuery(Guid CustomerId) : IRequest<List<PostQueryDTO>>;