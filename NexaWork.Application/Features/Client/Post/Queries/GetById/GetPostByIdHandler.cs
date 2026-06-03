using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace NexaWork.Application.Features.Client.Post.Queries.GetById;

public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, List<PostQueryDTO>>
{
    private readonly IPostRepository _postRepository;
    private readonly INexaWorkDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetPostByIdHandler(
        IPostRepository postRepository,
        INexaWorkDbContext context,
        ICurrentUserService currentUserService)
    {
        _postRepository = postRepository;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<PostQueryDTO>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepository.GetAllPostsByCustomerIdAsync(request.CustomerId, cancellationToken);


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
                post.UpdatedAt
            )).ToList();
    }
}