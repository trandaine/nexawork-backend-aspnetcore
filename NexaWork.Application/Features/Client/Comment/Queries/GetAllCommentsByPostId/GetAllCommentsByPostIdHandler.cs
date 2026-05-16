using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Comment.Queries.GetAllCommentsByPostId;

public class GetAllCommentsByPostIdHandler : IRequestHandler<GetAllCommentsByPostIdQuery, List<CommentQueryDTO>>
{
    private readonly ICommentRepository _commentRepository;

    public GetAllCommentsByPostIdHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<List<CommentQueryDTO>> Handle(GetAllCommentsByPostIdQuery request,
        CancellationToken cancellationToken)
    {
        var comments = await _commentRepository.GetAllCommentByPostIdAsync(request.PostId, cancellationToken);

        return comments.Select(comment => new CommentQueryDTO(
            comment.CommentId,
            comment.PostId,
            comment.CustomerId,
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt,
            comment.LikesCount
        )).ToList();
    }
}