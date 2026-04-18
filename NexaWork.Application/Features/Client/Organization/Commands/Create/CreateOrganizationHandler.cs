using MediatR;
using NexaWork.Application.Interfaces;

namespace NexaWork.Application.Features.Client.Organization.Commands.Create;

public class CreateOrganizationHandler : IRequestHandler<CreateOrganizationCommand, Guid>
{
    private readonly INexaWorkDbContext _context;
    public CreateOrganizationHandler(INexaWorkDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var entity = new NexaWork.Domain.Entities.Organization
        {
            OrganizationId = Guid.NewGuid(),
            Name = request.Name,
            Industry = request.Industry,
            Location = request.Location,
            Description = request.Description,
            WebsiteUrl = request.WebsiteUrl,
            OrganizationLogoUrl = request.OrganizationLogoUrl,
            FoundedDate = request.FoundedDate
        };

        _context.Organizations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.OrganizationId;
    }
}
