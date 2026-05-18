using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ISkillRepository
{
    /// <summary>
    /// Adds a new Skill to the repository.
    /// </summary>
    /// <param name="skill"></param>
    void Add(Skill skill);

    /// <summary>
    /// Retrieves all Skills.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Skill>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a Skill by its unique ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Update specific skill
    /// </summary>
    /// <param name="skill"></param>
    void Update(Skill skill);

    /// <summary>
    /// Delete specific skill
    /// </summary>
    /// <param name="skill"></param>
    void Remove(Skill skill);
}