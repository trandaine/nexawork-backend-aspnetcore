using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class CustomerSocialLinkRepository : ICustomerSocialLinkRepository
{
    private readonly NexaWorkDbContext _context;

    public CustomerSocialLinkRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public void Create(CustomerSocialLink customerSocialLink)
    {
        _context.CustomerSocialLinks.Add(customerSocialLink);
    }

    public void Update(CustomerSocialLink customerSocialLink)
    {
        _context.CustomerSocialLinks.Update(customerSocialLink);
    }

    public async Task<CustomerSocialLink?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.CustomerSocialLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId,
                cancellationToken);
    }

    public async Task<CustomerSocialLink?> GetByCustomerIdToEditAsync(Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.CustomerSocialLinks.FirstOrDefaultAsync(c => c.CustomerId == customerId,
            cancellationToken);
    }
}