using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Post.Queries;

public record PostQueryDTO
(
    Guid PostId,
    Guid CustomerId,
    string CustomerName,
    string? CustomerProfilePictureUrl,
    string Content,
    string? MediaUrl,
    int LikesCount,
    int CommentsCount,
    int SharesCount,
    VisibilityLevel Visibility,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
