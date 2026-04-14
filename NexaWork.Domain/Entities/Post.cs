using System.Collections.ObjectModel;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Post
{
    public Guid PostId { get; set; }
    public Guid CustomerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public VisibilityLevel Visibility { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<Comment> Comments { get; set; } = new Collection<Comment>();
    public virtual ICollection<Reaction> Reactions { get; set; } = new Collection<Reaction>();
}
