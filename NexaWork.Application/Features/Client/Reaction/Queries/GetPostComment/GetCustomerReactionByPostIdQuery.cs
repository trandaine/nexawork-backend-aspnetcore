using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Reaction.Queries.GetPostComment;

/// <summary>
/// Get the reaction status of the customer for a specific post by post id
/// </summary>
/// <param name="PostId"></param>
public record GetCustomerReactionByPostIdQuery(Guid PostId):IRequest<Boolean>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}