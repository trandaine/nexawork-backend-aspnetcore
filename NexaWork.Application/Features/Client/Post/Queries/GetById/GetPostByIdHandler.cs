using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NexaWork.Application.Features.Client.Post.Queries.GetById;

public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, List<PostQueryDTO>>
{
    private readonly IPostRepository _postRepository;
    private readonly INexaWorkDbContext _context;
    private readonly IConnectionRepository _connectionRepository;

    public GetPostByIdHandler(
        IPostRepository postRepository,
        INexaWorkDbContext context,
        IConnectionRepository connectionRepository)
    {
        _postRepository = postRepository;
        _context = context;
        _connectionRepository = connectionRepository;
    }

    public async Task<List<PostQueryDTO>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var currentCustomer =
            await _context.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == userIdentityId, cancellationToken);
        var friendIds = new HashSet<Guid>();

        List<NexaWork.Domain.Entities.Post> posts;

        if (currentCustomer != null)
        {
            if (request.CustomerId == currentCustomer.CustomerId)
            {
                posts = await _postRepository.GetAllPostsForMeAsync(currentCustomer.CustomerId, cancellationToken);
            }
            else
            {
                posts = await _postRepository.GetAllPostsByCustomerIdAsync(request.CustomerId, cancellationToken);
            }

            var connections =
                await _connectionRepository.GetConnectionsAsync(currentCustomer.CustomerId, cancellationToken);
            friendIds = connections
                .Select(c => c.CustomerId == currentCustomer.CustomerId ? c.ConnectedCustomerId : c.CustomerId)
                .ToHashSet();
        }
        else
        {
            posts = await _postRepository.GetAllPostsByCustomerIdAsync(request.CustomerId, cancellationToken);
        }


        return posts
            .Select(post => new PostQueryDTO(
                post.PostId,
                post.CustomerId,
                string.IsNullOrWhiteSpace(post.Customer.FirstName) && string.IsNullOrWhiteSpace(post.Customer.LastName)
                    ? "Anonymous User" // If both are null
                    : (post.Customer.FirstName + " " + post.Customer.LastName)
                    .Trim(), // Trim to remove any extra space if one of them is null
                post.Customer.ProfilePictureUrl,
                post.Content,
                post.MediaUrl,
                post.LikesCount,
                post.CommentsCount,
                post.SharesCount,
                post.Visibility,
                post.CreatedAt,
                post.UpdatedAt,
                // currentCustomer != null && post.CustomerId == currentCustomer.CustomerId ? true : friendIds.Contains(post.CustomerId)
                (currentCustomer != null
                 && post.CustomerId == currentCustomer.CustomerId)
                || friendIds.Contains(post.CustomerId)
            )).ToList();
    }
}