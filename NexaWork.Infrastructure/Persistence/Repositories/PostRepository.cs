using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class PostRepository : IPostRepository
{
    private readonly NexaWorkDbContext _context;

    public PostRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public void Add(Post post)
    {
        _context.Posts.Add(post);
    }

    public async Task<List<Post>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Posts
            .Include(p => p.Customer)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .Include(p => p.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PostId == id, cancellationToken);
    }

    public void Update(Post post)
    {
        _context.Posts.Update(post);
    }

    public void Remove(Post post)
    {
        _context.Posts.Remove(post);
    }
}
