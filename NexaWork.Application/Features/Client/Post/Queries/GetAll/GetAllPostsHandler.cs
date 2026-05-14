using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;


namespace NexaWork.Application.Features.Client.Post.Queries.GetAll;

public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, List<PostQueryDTO>>
{
    private readonly IPostRepository _postRepository;
    private readonly INexaWorkDbContext _context;

    public GetAllPostsHandler(
        IPostRepository postRepository,
        INexaWorkDbContext context
    )
    {
        _postRepository = postRepository;
        _context = context;
    }
    public async Task<List<PostQueryDTO>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        // var posts = await _postRepository.GetAllAsync(cancellationToken);

        // return posts.Select(posts => new PostQueryDTO
        // (
        //     posts.PostId,
        //     $"{posts.Customer.FirstName} {posts.Customer.LastName}",
        //     posts.Content,
        //     posts.MediaUrl,
        //     posts.LikesCount,
        //     posts.CommentsCount,
        //     posts.SharesCount,
        //     posts.Visibility,
        //     posts.CreatedAt,
        //     posts.UpdatedAt
        // )).ToList();


        // Instead of fetching all posts and then mapping them in memory, we can directly project the data into PostQueryDTO using LINQ. 
        // This way, we only retrieve the necessary fields from the database, which can significantly improve performance, especially when dealing with a large number of posts.
        return await _context.Posts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(post => new PostQueryDTO
            (
                post.PostId,
                // $"{post.Customer.FirstName} {post.Customer.LastName}",
                string.IsNullOrWhiteSpace(post.Customer.FirstName) && string.IsNullOrWhiteSpace(post.Customer.LastName)
                    ? "Anonymous User" // If both are null
                    : (post.Customer.FirstName + " " + post.Customer.LastName).Trim(), // Trim to remove any extra space if one of them is null
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
