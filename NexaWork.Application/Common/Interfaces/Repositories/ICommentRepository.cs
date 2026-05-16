using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ICommentRepository
{
    /// <summary>
    /// Add new comment
    /// </summary>
    /// <param name="comment"></param>
    void Add(Comment comment);

    /// <summary>
    /// Update existed comment
    /// </summary>
    /// <param name="comment"></param>
    void Update(Comment comment);

    /// <summary>
    /// Remove existed comment
    /// </summary>
    /// <param name="comment"></param>
    void Remove(Comment comment);
    
    /// <summary>
    /// Get comment by CommentId to Edit, or Delete
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Get all comments by PostId
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Comment>> GetAllCommentByPostIdAsync(Guid postId, CancellationToken cancellationToken);
}