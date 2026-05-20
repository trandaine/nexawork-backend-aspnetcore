using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface IPostRepository
{
    /// <summary>
    /// Adds a new Post to the repository.
    /// </summary>
    /// <param name="post"></param>
    void Add(Post post);

    /// <summary>
    /// Retrieves all Posts.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Post>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a Post by its unique ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Post?> GetByIdForEditAsync(Guid id, CancellationToken cancellationToken);

    void Update(Post post);

    void Remove(Post post);
}
