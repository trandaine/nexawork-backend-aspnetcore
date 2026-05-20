using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ICustomerAddressRepository
{
    void Create(CustomerAddress customerAddress);

    Task<CustomerAddress?> GetByCustomerAddressIdAsync(Guid customerAddressId, CancellationToken cancellationToken);

    Task<CustomerAddress?> GetByCustomerAddressIdToEditAsync(Guid customerAddressId,
        CancellationToken cancellationToken);
}