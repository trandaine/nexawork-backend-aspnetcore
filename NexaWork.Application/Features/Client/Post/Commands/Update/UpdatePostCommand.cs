using NexaWork.Application.Common.Interfaces;
using MediatR;
using NexaWork.Application.DTOs;
// using NexaWork.Application.DTOs.Post;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Post.Commands.Update;

public record UpdatePostCommand(
    Guid PostId,
    string Content,
    FileDTO? MediaFile,
    VisibilityLevel Visibility
) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}