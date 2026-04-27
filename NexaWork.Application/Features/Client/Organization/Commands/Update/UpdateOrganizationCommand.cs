using System;
using MediatR;

namespace NexaWork.Application.Features.Client.Organization.Commands.Update;

// public class UpdateOrganizationCommand
// {

// }

public record UpdateOrganizationCommand(
    Guid OrganizationId,
    string Name,
    string? Industry,
    string? Location,
    string? Description,
    string? WebsiteUrl,
    string? LogoUrl,
    DateTime? FoundedDate) : IRequest;
