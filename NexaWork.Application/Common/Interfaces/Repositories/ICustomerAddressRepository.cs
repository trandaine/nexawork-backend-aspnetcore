using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ICustomerAddressRepository
{
    void Create(CustomerAddress customerAddress);

    /// <summary>
    /// Get record by CustomerId
    /// </summary>
    /// <param name="customerAddressId">CustomerId</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CustomerAddress?> GetByCustomerAddressIdAsync(Guid customerAddressId, CancellationToken cancellationToken);

    /// <summary>
    /// Get record by CustomerId for edit
    /// </summary>
    /// <param name="customerAddressId">CustomerId</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CustomerAddress?> GetByCustomerAddressIdToEditAsync(Guid customerAddressId,
        CancellationToken cancellationToken);
}