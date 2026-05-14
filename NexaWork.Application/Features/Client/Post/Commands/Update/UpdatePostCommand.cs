
using MediatR;
using NexaWork.Application.DTOs.Post;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Post.Commands.Update;

public record UpdatePostCommand
(
    Guid PostId,
    string IdentityUserId,
    string Content,
    FileDTO? MediaFile,
    VisibilityLevel Visibility
) : IRequest;
