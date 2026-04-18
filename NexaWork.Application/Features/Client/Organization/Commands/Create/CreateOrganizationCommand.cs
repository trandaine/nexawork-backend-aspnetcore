using MediatR;

namespace NexaWork.Application.Features.Client.Organization.Commands.Create;

public class CreateOrganizationCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Industry { get; init; }
    public string? Location { get; init; }
    public string? Description { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? OrganizationLogoUrl { get; init; }
    public DateTime? FoundedDate { get; init; }
}
