using System;
using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Organization.Commands.Update;

public class UpdateOrganizationHandler : IRequestHandler<UpdateOrganizationCommand>
{
    private readonly IOrganizationRepository _repository;
    private readonly INexaWorkDbContext _unitOfWork;

    public UpdateOrganizationHandler(
        IOrganizationRepository repository,
        INexaWorkDbContext unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var editOrganization = await _repository.GetByIdToUpdateAsync(request.OrganizationId, cancellationToken);

        if (editOrganization == null)
        {
            throw new Exception($"Organization with ID {request.OrganizationId} not found");
        }

        editOrganization.Update(
            request.Name,
            request.Industry,
            request.Location,
            request.Description,
            request.WebsiteUrl,
            request.LogoUrl,
            request.FoundedDate
        );
        // _repository.Update(editOrganization);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }
}
