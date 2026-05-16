using System.Collections.ObjectModel;

namespace NexaWork.Domain.Entities;

public class Comment
{
    public Guid CommentId { get; private set; }
    public Guid PostId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public int LikesCount { get; private set; }


    public Post Post { get; private set; } = null!;
    public virtual Customer Customer { get; private set; } = null!;
    public virtual ICollection<Reaction> Reactions { get; private set; } = new Collection<Reaction>();
    
    
    private Comment() { }
    public static Comment Create(Guid postId,Guid customerId, string content)
    {
        bool hasContent = !string.IsNullOrWhiteSpace(content);
        if(!hasContent)
        {
            throw new ArgumentException("Comment content cannot be empty.");
        }
        return new Comment()
        {
            CommentId = Guid.NewGuid(),
            PostId = postId,
            CustomerId = customerId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            LikesCount = 0
        };
    }
    
    public void Update(string content)
    {
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }
}