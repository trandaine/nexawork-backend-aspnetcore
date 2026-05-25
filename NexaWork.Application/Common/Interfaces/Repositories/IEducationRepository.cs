using NexaWork.Application.Features.Client.Education.Commands.Create;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface IEducationRepository
{
    void Create(Education education);
    Task<Education?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<Education?> GetByCustomerIdToEditAsync(Guid customerId, CancellationToken cancellationToken);
}