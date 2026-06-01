using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;



namespace NexaWork.Application.Features.Client.Post.Queries.GetById;

public record GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, PostQueryDTO?>
{
    private readonly IPostRepository _postRepository;

    public GetPostByIdHandler(
        IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostQueryDTO?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) return null;

        return new PostQueryDTO(
            post.PostId,
            string.IsNullOrWhiteSpace(post.Customer.FirstName) && string.IsNullOrWhiteSpace(post.Customer.LastName)
                ? "Anonymous User" // If both are null
                : (post.Customer.FirstName + " " + post.Customer.LastName).Trim(), // Trim to remove any extra space if one of them is null

            string.IsNullOrEmpty(post.Customer.ProfilePictureUrl) ? null : post.Customer.ProfilePictureUrl,
            post.Content,
            post.MediaUrl,
            post.LikesCount,
            post.CommentsCount,
            post.SharesCount,
            post.Visibility,
            post.CreatedAt,
            post.UpdatedAt
        );
    }
}

