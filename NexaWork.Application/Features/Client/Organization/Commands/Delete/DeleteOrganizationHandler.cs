using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Organization.Commands.Delete;

public class DeleteOrganizationHandler : IRequestHandler<DeleteOrganizationCommand>
{
    private readonly IOrganizationRepository _repository;
    private readonly INexaWorkDbContext _unitOfWork;

    public DeleteOrganizationHandler(
        IOrganizationRepository repository,
        INexaWorkDbContext unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _repository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            throw new Exception($"Organization with ID {request.OrganizationId} not found");
        }

        _repository.Remove(organization);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }
}
