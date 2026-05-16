using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces;

public interface INexaWorkDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Post> Posts { get; }
    DbSet<Comment> Comments { get; }

    /// <summary>
    /// Lưu tất cả các thay đổi vào database. Trả về số lượng bản ghi bị ảnh hưởng.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
