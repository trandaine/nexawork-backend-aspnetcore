namespace NexaWork.Application.Features.Client.CustomerSocialLink.Queries;

public record CustomerSocialLinkQueryDTO(
    string? FaceBookUrl,
    string? LinkedInUrl,
    string? XUrl,
    string? InstagramUrl
);