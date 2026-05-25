using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Features.Client.Education.Commands.Create;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class EducationRepository : IEducationRepository
{
    private readonly NexaWorkDbContext _context;

    public EducationRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public void Create(Education education)
    {
        _context.Educations.Add(education);
    }

    public async Task<Education?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Educations
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.CustomerId == customerId, cancellationToken);
    }

    public async Task<Education?> GetByCustomerIdToEditAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Educations
            .SingleOrDefaultAsync(e => e.CustomerId == customerId, cancellationToken);
    }
}