using System;
using System.Runtime.CompilerServices;
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




}
