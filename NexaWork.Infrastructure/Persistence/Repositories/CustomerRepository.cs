using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace NexaWork.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly NexaWorkDbContext _context;
    public CustomerRepository(NexaWorkDbContext context)
    {
        _context = context;
    }
    public async Task<Customer?> GetByIdentityIdAsync(string identityUserId, CancellationToken cancellationToken)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId, cancellationToken);
    }
}
