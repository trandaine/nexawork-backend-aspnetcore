using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;

public record CreateCustomerSocialLinkCommand(
    string? FaceBookUrl,
    string? LinkedInUrl,
    string? XUrl,
    string? InstagramUrl ):IRequest<Guid>;