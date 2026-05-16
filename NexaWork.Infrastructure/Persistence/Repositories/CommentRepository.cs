using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly NexaWorkDbContext _context;
    public CommentRepository(NexaWorkDbContext context)
    {
        _context = context;
    }
    
    
    public void Add(Comment comment)
    {
        _context.Comments.Add(comment);
    }

    public void Update(Comment comment)
    {
        _context.Comments.Update(comment);
    }

    public void Remove(Comment comment)
    {
        _context.Comments.Remove(comment);
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .FirstOrDefaultAsync(o => o.CommentId == id, cancellationToken);
    }

    public async Task<List<Comment>> GetAllCommentByPostIdAsync(Guid postId, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.PostId == postId)
            .ToListAsync(cancellationToken);
    }
}