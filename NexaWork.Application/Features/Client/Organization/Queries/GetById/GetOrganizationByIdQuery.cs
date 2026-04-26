using System;
using MediatR;

namespace NexaWork.Application.Features.Client.Organization.Queries.GetById;

// public class GetOrganizationByIdQuery
// {

// }
public record GetOrganizationByIdQuery(Guid Id) : IRequest<OrganizationQueryDto?>;
