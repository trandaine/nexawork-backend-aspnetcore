using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Commands.Create;

public record CreateCustomerCommand(
    string IdentityUserId
) : IRequest<Guid>;



// {
//     public string Name { get; init; } = string.Empty;
//     public string? Industry { get; init; }
//     public string? Location { get; init; }
//     public string? Description { get; init; }
//     public string? WebsiteUrl { get; init; }
//     public string? OrganizationLogoUrl { get; init; }
//     public DateTime? FoundedDate { get; init; }
// }
