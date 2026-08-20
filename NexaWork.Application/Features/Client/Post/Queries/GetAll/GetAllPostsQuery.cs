using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Post.Queries.GetAll;

/// <summary>
/// Get all personal posts to display on feed for user 
/// </summary>
public record GetAllPostsQuery() : IRequest<List<PostQueryDTO>>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
