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
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);


    /// <summary>
    /// Retrieves all Organizations.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken);

}
