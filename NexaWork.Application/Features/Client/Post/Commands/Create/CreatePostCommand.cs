using MediatR;
using NexaWork.Application.DTOs.Post;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Post.Commands.Create;

/// <summary>
/// Represents a command to create a new post.
/// </summary>
/// <param name="CustomerId">The ID of the customer creating the post.</param>
/// <param name="Content">The content of the post.</param>
/// <param name="MediaFile">The file data for any media attached to the post.</param>
/// <param name="Visibility">The visibility level of the post.</param>
public record CreatePostCommand(
    string IdentityUserId,
    Guid CustomerId,
    string Content,
    FileDTO? MediaFile,
    // string? MediaUrl,
    VisibilityLevel Visibility
) : IRequest<Guid>;
