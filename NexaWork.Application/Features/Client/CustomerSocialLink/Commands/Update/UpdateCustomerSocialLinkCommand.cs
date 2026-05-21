using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Update;

public record UpdateCustomerSocialLinkCommand(
    string? FaceBookUrl,
    string? LinkedInUrl,
    string? XUrl,
    string? InstagramUrl) : IRequest;