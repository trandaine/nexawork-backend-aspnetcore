using System;
using System.Collections.ObjectModel;

namespace NexaWork.Domain.Entities;

public class Comment
{
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public Guid CustomerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int LikesCount { get; set; }

    
    public Post Post { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<Reaction> Reactions { get; set; } = new Collection<Reaction>();
}
