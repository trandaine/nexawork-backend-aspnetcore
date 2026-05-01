using System;
using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Organization.Queries.GetById;

public class GetOrganizationByIdHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationQueryDTO?>
{
    private readonly IOrganizationRepository _repository;
    public GetOrganizationByIdHandler(IOrganizationRepository repository)
    {
        _repository = repository;
    }
    public async Task<OrganizationQueryDTO?> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (organization == null)
            return null;

        return new OrganizationQueryDTO(
            organization.OrganizationId,
            organization.Name,
            organization.Industry,
            organization.Location,
            organization.Description,
            organization.WebsiteUrl,
            organization.OrganizationLogoUrl,
            organization.FoundedDate
        );

    }
}

