using System.Collections.ObjectModel;
using System.Net.Mime;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Post
{
    public Guid PostId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? MediaUrl { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // Post engagement properties
    public int LikesCount { get; private set; }
    public int CommentsCount { get; private set; }
    public int SharesCount { get; private set; }

    public VisibilityLevel Visibility { get; private set; }

    public virtual Customer Customer { get; private set; } = null!;
    public virtual ICollection<Comment> Comments { get; private set; } = new Collection<Comment>();
    public virtual ICollection<Reaction> Reactions { get; private set; } = new Collection<Reaction>();

    private Post()
    {
        // Required by EF Core
    }

    public static Post Create(Guid customerId, string content, string? mediaUrl, VisibilityLevel visibility)
    {
        // A post cannot be completely empty without content or media
        bool hasContent = !string.IsNullOrWhiteSpace(content);
        bool hasMedia = !string.IsNullOrWhiteSpace(mediaUrl);
        if (!hasContent && !hasMedia)
        {
            throw new ArgumentException("Post must have either content or media.");
        }

        return new Post
        {
            PostId = Guid.NewGuid(),
            CustomerId = customerId,
            Content = content,
            MediaUrl = mediaUrl,
            Visibility = visibility,
            CreatedAt = DateTime.UtcNow,

            LikesCount = 0,
            CommentsCount = 0,
            SharesCount = 0
        };
    }

    public void Update(string content, string? newMediaUrl, VisibilityLevel visibility)
    {
        Content = content;
        Visibility = visibility;
        UpdatedAt = DateTime.UtcNow;
        if (newMediaUrl != null)
        {
            MediaUrl = newMediaUrl;
        }
    }
    
    public void UpdateLikesCount()
    {
        LikesCount++;
    }

    public void DecrementLikesCount()
    {
        LikesCount--;
    }

    // public void Delete()
    // {

    // }
}