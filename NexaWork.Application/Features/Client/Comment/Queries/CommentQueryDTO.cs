namespace NexaWork.Application.Features.Client.Comment.Queries;

public record CommentQueryDTO(
    Guid CommentId,
    Guid PostId,
    Guid CustomerId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int LikesCount);