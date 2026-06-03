using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

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


    public async Task<List<Post>> GetAllAsync(Guid currentCustomerId, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .Include(p => p.Customer)
            // .AsNoTracking()
            // .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .Where(post =>
                post.Visibility == VisibilityLevel.Public ||
                post.CustomerId == currentCustomerId ||
                (post.Visibility == VisibilityLevel.Connections &&
                 _context.Connections.Any(conn =>
                     conn.Status == ConnectionStatus.Accepted &&
                     ((conn.CustomerId == post.CustomerId && conn.ConnectedCustomerId == currentCustomerId) ||
                      (conn.CustomerId == currentCustomerId && conn.ConnectedCustomerId == post.CustomerId))
                 ))
            )
            //.Include(async post  => post.CustomerId == await _customerRepository.GetCustomerByIdAsync(post.CustomerId, cancellationToken))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetAllPostsForMeAsync(Guid myId, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .AsNoTracking()
            .Where(post => post.CustomerId == myId)
            .Include(p => p.Customer)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetAllPostsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .AsNoTracking()
            .Include(p => p.Customer)
            .Where(post => post.CustomerId == customerId && post.Visibility == VisibilityLevel.Public)
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

    public async Task<Post?> GetByIdForEditAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .Include(p => p.Customer)
            // .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PostId == id, cancellationToken);
    }

    public void Update(Post post)
    {
        _context.Posts.Update(post);
    }

    public void UpdateLikesCount(int likeCounts)
    {
        throw new NotImplementedException();
    }

    public void Remove(Post post)
    {
        _context.Posts.Remove(post);
    }
}