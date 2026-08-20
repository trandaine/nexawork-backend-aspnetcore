using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Post.Queries.GetAllCurrentCustomer;

public class GetAllCurrentCustomerHandler : IRequestHandler<GetAllCurrentCustomerQuery, List<PostQueryDTO>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPostRepository _postRepository;
    public GetAllCurrentCustomerHandler(
        
        IPostRepository postRepository,
        ICustomerRepository customerRepository
    )
    {
        _postRepository = postRepository;
        _customerRepository = customerRepository;
    }
    public async Task<List<PostQueryDTO>> Handle(GetAllCurrentCustomerQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var currentCustomer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (currentCustomer == null)
            throw new UnauthorizedAccessException(
                "This request cannot be processed without authentication. Please log in to continue.");

        var posts = await _postRepository.GetAllPostsForMeAsync(currentCustomer.CustomerId, cancellationToken);

        return posts
            .Select(post => new PostQueryDTO
            (
                post.PostId,
                post.CustomerId,
                string.IsNullOrWhiteSpace(post.Customer?.FirstName) && string.IsNullOrWhiteSpace(post.Customer?.LastName)
                    ? "Anonymous User" // If both are null
                    : (post.Customer?.FirstName + " " + post.Customer?.LastName)
                    .Trim(), // Trim to remove any extra space if one of them is null
                string.IsNullOrEmpty(post.Customer?.ProfilePictureUrl) ? null : post.Customer?.ProfilePictureUrl,
                //customerAvatar,
                post.Content,
                post.MediaUrl,
                post.LikesCount,
                post.CommentsCount,
                post.SharesCount,
                post.Visibility,
                post.CreatedAt,
                post.UpdatedAt,
                true
            ))
            .ToList();
    }
}