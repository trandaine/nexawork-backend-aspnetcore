using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Update;

public record UpdateCustomerSocialLinkCommand(
    string? FaceBookUrl,
    string? LinkedInUrl,
    string? XUrl,
    string? InstagramUrl) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}