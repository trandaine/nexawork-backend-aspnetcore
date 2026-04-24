using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

internal class OrganizationRepository : IOrganizationRepository
{
    private readonly NexaWorkDbContext _context;

    public OrganizationRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public void Add(Organization organization)
    {
        // Simply adds the aggregate root to EF Core's memory tracking.
        // It does NOT save to the database here.
        _context.Organizations.Add(organization);
    }

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Organizations
            // We use AsNoTracking() here because this method is currently 
            // designed for the Read (Query) side of CQRS.
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrganizationId == id, cancellationToken);
    }
}
