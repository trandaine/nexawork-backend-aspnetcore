using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class CustomerAddressRepository : ICustomerAddressRepository
{
    private readonly NexaWorkDbContext _context;

    public CustomerAddressRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public void Create(CustomerAddress customerAddress)
    {
        _context.CustomerAddresses.Add(customerAddress);
    }

    public async Task<CustomerAddress?> GetByCustomerAddressIdAsync(Guid customerAddressId,
        CancellationToken cancellationToken)
    {
        return await _context.CustomerAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.CustomerId == customerAddressId, cancellationToken);
    }

    public async Task<CustomerAddress?> GetByCustomerAddressIdToEditAsync(Guid customerAddressId,
        CancellationToken cancellationToken)
    {
        return await _context.CustomerAddresses
            .FirstOrDefaultAsync(o => o.CustomerId == customerAddressId, cancellationToken);
    }
}