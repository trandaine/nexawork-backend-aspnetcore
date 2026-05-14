using MediatR;

namespace NexaWork.Application.Features.Client.Organization.Queries.GetAll;

// public class GetAllOrganizationQuery
// {

// }

public record GetAllOrganizationsQuery() : IRequest<List<OrganizationQueryDTO>>;
