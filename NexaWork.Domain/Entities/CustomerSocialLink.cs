namespace NexaWork.Domain.Entities;

public class CustomerSocialLink
{
    public Guid CustomerSocialLinkId { get; private set; }
    public string? FaceBookUrl { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? XUrl { get; private set; }
    public string? InstagramUrl { get; private set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public Guid CustomerId { get; private set; }
    
    
    // Navigation property
    public Customer Customer { get; private set; } = null!;
}