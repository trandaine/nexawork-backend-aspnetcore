using System;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface IOrganizationRepository
{
    /// <summary>
    /// Adds a new Organization to the repository.
    /// Note: This does not save to the database. Saving is handled by the Unit of Work.
    /// </summary>
    /// <param name="organization"></param>
    void Add(Organization organization);



    /// <summary>
    /// Retrieves an Organization by its unique ID.
    /// Note: The implementation should use .AsNoTracking() since this is for read-only operations.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
