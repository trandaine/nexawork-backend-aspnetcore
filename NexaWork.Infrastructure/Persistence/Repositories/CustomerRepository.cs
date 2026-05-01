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

    public void Create(Customer customer)
    {
        _context.Customers.Add(customer);
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Customers
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.CustomerId == id, cancellationToken);
    }


    public async Task<Customer?> GetByIdentityIdAsync(string identityUserId, CancellationToken cancellationToken)
    {
        return await _context.Customers
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId, cancellationToken);
    }

    public async Task<List<Customer>> GetAllCustomerAsync(CancellationToken cancellationToken)
    {
        return await _context.Customers
        .AsNoTracking()
        .ToListAsync(cancellationToken);
    }
}
