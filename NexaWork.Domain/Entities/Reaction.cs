using System.ComponentModel;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Reaction
{
    public Guid ReactionId { get; set; }
    public Guid CustomerId { get; set; }

    [Description("Either PostId or CommentId should be set, but not both.")]
    public Guid? PostId { get; set; }
    [Description("Either PostId or CommentId should be set, but not both.")]
    public Guid? CommentId { get; set; }
    
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Customer Customer { get; set; } = null!;
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
}
