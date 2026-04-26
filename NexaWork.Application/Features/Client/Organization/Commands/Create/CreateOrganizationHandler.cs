using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Organization.Commands.Create;

public class CreateOrganizationHandler : IRequestHandler<CreateOrganizationCommand, Guid>
{
    #region Old Code
    // private readonly INexaWorkDbContext _context;
    // public CreateOrganizationHandler(INexaWorkDbContext context)
    // {
    //     _context = context;
    // }

    // public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    // {
    //     var entity = new NexaWork.Domain.Entities.Organization
    //     {
    //         OrganizationId = Guid.NewGuid(),
    //         Name = request.Name,
    //         Industry = request.Industry,
    //         Location = request.Location,
    //         Description = request.Description,
    //         WebsiteUrl = request.WebsiteUrl,
    //         OrganizationLogoUrl = request.OrganizationLogoUrl,
    //         FoundedDate = request.FoundedDate
    //     };

    //     _context.Organizations.Add(entity);
    //     await _context.SaveChangesAsync(cancellationToken);
    //     return entity.OrganizationId;
    // }
    #endregion

    private readonly IOrganizationRepository _repository;
    private readonly INexaWorkDbContext _unitOfWork;

    public CreateOrganizationHandler(
        IOrganizationRepository repository, 
        INexaWorkDbContext unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        // 1. Create the entity (Input is guaranteed to be valid here)
        // var organization = Organization.Create(
        //     request.Name,
        //     request.Industry,
        //     request.Location,
        //     request.Description,
        //     request.WebsiteUrl,
        //     request.OrganizationLogoUrl,
        //     request.FoundedDate
        // );

        var organization = NexaWork.Domain.Entities.Organization.Create(
            request.Name,
            request.Industry,
            request.Location,
            request.Description,
            request.WebsiteUrl,
            request.OrganizationLogoUrl,
            request.FoundedDate
        );

        // 2. Track in memory
        _repository.Add(organization);

        // 3. Persist to database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organization.OrganizationId;
    }
}
