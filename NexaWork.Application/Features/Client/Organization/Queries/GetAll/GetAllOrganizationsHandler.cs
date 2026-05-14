using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Organization.Queries.GetAll;

public class GetAllOrganizationsHandler : IRequestHandler<GetAllOrganizationsQuery, List<OrganizationQueryDTO>>
{
    private readonly IOrganizationRepository _repository;
    public GetAllOrganizationsHandler(IOrganizationRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<OrganizationQueryDTO>> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await _repository.GetAllAsync(cancellationToken);

        return organizations.Select(organizations => new OrganizationQueryDTO(
            organizations.OrganizationId,
            organizations.Name,
            organizations.Industry,
            organizations.Location,
            organizations.Description,
            organizations.WebsiteUrl,
            organizations.OrganizationLogoUrl,
            organizations.FoundedDate
        )).ToList();

    }
}

