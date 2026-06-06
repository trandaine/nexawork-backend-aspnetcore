using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;


namespace NexaWork.Application.Features.Client.Post.Queries.GetAll;

public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, List<PostQueryDTO>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPostRepository _postRepository;
    private readonly IConnectionRepository _connectionRepository;

    public GetAllPostsHandler(
        ICurrentUserService currentUserService,
        IPostRepository postRepository,
        ICustomerRepository customerRepository,
        IConnectionRepository connectionRepository
    )
    {
        _postRepository = postRepository;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
        _connectionRepository = connectionRepository;
    }

    public async Task<List<PostQueryDTO>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
        var currentCustomer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (currentCustomer == null)
            throw new UnauthorizedAccessException(
                "This request cannot be processed without authentication. Please log in to continue.");

        var posts = await _postRepository.GetAllAsync(currentCustomer.CustomerId, cancellationToken);

        var connections = await _connectionRepository.GetConnectionsAsync(currentCustomer.CustomerId, cancellationToken);
        var friendIds = connections.Select(c => c.CustomerId == currentCustomer.CustomerId ? c.ConnectedCustomerId : c.CustomerId).ToHashSet();

        return posts
            .Select(post => new PostQueryDTO
            (
                post.PostId,
                post.CustomerId,
                string.IsNullOrWhiteSpace(post.Customer.FirstName) && string.IsNullOrWhiteSpace(post.Customer.LastName)
                    ? "Anonymous User" // If both are null
                    : (post.Customer.FirstName + " " + post.Customer.LastName)
                    .Trim(), // Trim to remove any extra space if one of them is null
                string.IsNullOrEmpty(post.Customer.ProfilePictureUrl) ? null : post.Customer.ProfilePictureUrl,
                //customerAvatar,
                post.Content,
                post.MediaUrl,
                post.LikesCount,
                post.CommentsCount,
                post.SharesCount,
                post.Visibility,
                post.CreatedAt,
                post.UpdatedAt,
                // if the post owner is belonged to the current user set true, if it is other user post, if not friend (false), if friend (true) 
                post.CustomerId == currentCustomer.CustomerId || friendIds.Contains(post.CustomerId)
            ))
            .ToList();
    }
}

// 