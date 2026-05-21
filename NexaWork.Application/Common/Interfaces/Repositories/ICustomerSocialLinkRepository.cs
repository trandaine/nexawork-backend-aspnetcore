using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ICustomerSocialLinkRepository
{
    void Create(CustomerSocialLink customerSocialLink);
    
    void Update(CustomerSocialLink customerSocialLink);

    Task<CustomerSocialLink?> GetByCustomerIdAsync(Guid customerId,
        CancellationToken cancellationToken);

    Task<CustomerSocialLink?> GetByCustomerIdToEditAsync(Guid customerId,
        CancellationToken cancellationToken);
}