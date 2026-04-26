using System;

namespace NexaWork.Application.Features.Client.Organization.Queries;

public record OrganizationQueryDto
(
    Guid OrganizationId,
    string Name,
    string? Industry,
    string? Location,
    string? Description,
    string? WebsiteUrl,
    string? OrganizationLogoUrl,
    DateTime? FoundedDate
);
