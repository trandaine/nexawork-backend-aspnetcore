using System.ComponentModel;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Reaction
{
    public Guid ReactionId { get; private set; }
    public Guid CustomerId { get; private set; }

    [Description("Either PostId or CommentId should be private set, but not both.")]
    public Guid? PostId { get; private set; }
    [Description("Either PostId or CommentId should be private set, but not both.")]
    public Guid? CommentId { get; private set; }
    
    public ReactionType ReactionType { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual Customer Customer { get; private set; } = null!;
    public virtual Post? Post { get; private set; }
    public virtual Comment? Comment { get; private set; }

    public Reaction()
    {
    }
    
    public static Reaction CreateReacTionForPost(Guid customerId, Guid postId)
    {
        return new Reaction
        {
            ReactionId = Guid.NewGuid(),
            CustomerId = customerId,
            PostId = postId,
            CommentId = null,
            ReactionType = ReactionType.Love,
            CreatedAt = DateTime.UtcNow
        };
    }
}
