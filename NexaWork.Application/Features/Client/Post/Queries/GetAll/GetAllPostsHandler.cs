using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Enums;


namespace NexaWork.Application.Features.Client.Post.Queries.GetAll;

public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, List<PostQueryDTO>>
{
    private readonly IPostRepository _postRepository;
    private readonly INexaWorkDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerRepository _customerRepository;

    public GetAllPostsHandler(
        IPostRepository postRepository,
        INexaWorkDbContext context,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService
    )
    {
        _customerRepository = customerRepository;
        _postRepository = postRepository;
        _context = context;
        _currentUserService = currentUserService;
    }
    public async Task<List<PostQueryDTO>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        


        // Instead of fetching all posts and then mapping them in memory, we can directly project the data into PostQueryDTO using LINQ. 
        // This way, we only retrieve the necessary fields from the database, which can significantly improve performance, especially when dealing with a large number of posts.
        // return await _context.Posts
        //     .AsNoTracking()
        //     .OrderByDescending(p => p.CreatedAt)
        //     .Select(post => new PostQueryDTO
        //     (
        //         post.PostId,
        //         // $"{post.Customer.FirstName} {post.Customer.LastName}",
        //         string.IsNullOrWhiteSpace(post.Customer.FirstName) && string.IsNullOrWhiteSpace(post.Customer.LastName)
        //             ? "Anonymous User" // If both are null
        //             : (post.Customer.FirstName + " " + post.Customer.LastName).Trim(), // Trim to remove any extra space if one of them is null
        //         post.Content,
        //         post.MediaUrl,
        //         post.LikesCount,
        //         post.CommentsCount,
        //         post.SharesCount,
        //         post.Visibility,
        //         post.CreatedAt,
        //         post.UpdatedAt
        //     ))
        //     .ToListAsync(cancellationToken);
        
        var userIdentityId = _currentUserService.UserId;
        var currentCustomer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        var currentCustomerId = currentCustomer?.CustomerId ?? Guid.Empty;
        
        return await _context.Posts
            .AsNoTracking()
            .Where(post =>
                post.Visibility == VisibilityLevel.Public ||
                post.CustomerId == currentCustomerId ||
                (post.Visibility == VisibilityLevel.Connections &&
                 _context.Connections.Any(conn =>
                     conn.Status == ConnectionStatus.Accepted &&
                     ((conn.CustomerId == post.CustomerId && conn.ConnectedCustomerId == currentCustomerId) ||
                      (conn.CustomerId == currentCustomerId && conn.ConnectedCustomerId == post.CustomerId))
                 ))
            )
            //.Include(async post  => post.CustomerId == await _customerRepository.GetCustomerByIdAsync(post.CustomerId, cancellationToken))
            .OrderByDescending(p => p.CreatedAt)
            .Select(post => new PostQueryDTO
            (
                post.PostId,
                string.IsNullOrWhiteSpace(post.Customer.FirstName) && string.IsNullOrWhiteSpace(post.Customer.LastName)
                    ? "Anonymous User" // If both are null
                    : (post.Customer.FirstName + " " + post.Customer.LastName).Trim(), // Trim to remove any extra space if one of them is null

                //post.Customer.ProfilePictureUrl,
                string.IsNullOrEmpty(post.Customer.ProfilePictureUrl) ? null : post.Customer.ProfilePictureUrl,
                //customerAvatar,
                post.Content,
                post.MediaUrl,
                post.LikesCount,
                post.CommentsCount,
                post.SharesCount,
                post.Visibility,
                post.CreatedAt,
                post.UpdatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}

// 
