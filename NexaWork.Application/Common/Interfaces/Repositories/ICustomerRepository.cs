using System;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdentityIdAsync(string identityUserId, CancellationToken cancellationToken);
}
