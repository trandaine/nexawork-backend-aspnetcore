using MediatR;

namespace NexaWork.Application.Features.Client.Organization.Commands.Delete;

public record DeleteOrganizationCommand(Guid OrganizationId) : IRequest;
