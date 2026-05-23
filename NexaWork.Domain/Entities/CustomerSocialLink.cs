namespace NexaWork.Domain.Entities;

public class CustomerSocialLink
{
    public Guid CustomerSocialLinkId { get; private set; }
    public string? FaceBookUrl { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? XUrl { get; private set; }
    public string? InstagramUrl { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime? DateUpdated { get; private set; }
    public Guid CustomerId { get; private set; }


    // Navigation property
    public Customer Customer { get; private set; } = null!;

    private CustomerSocialLink()
    {
    }
    
    

    public static CustomerSocialLink Create(Guid customerId)
    {
        return new CustomerSocialLink()
        {
            CustomerSocialLinkId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            CustomerId = customerId
        };
    }

    public void Update(string? faceBookUrl, string? linkedInUrl, string? xUrl, string? instagramUrl)
    {
        FaceBookUrl = UpdateSocialLinks(faceBookUrl);
        LinkedInUrl = UpdateSocialLinks(linkedInUrl);
        XUrl = UpdateSocialLinks(xUrl);
        InstagramUrl = UpdateSocialLinks(instagramUrl);
        DateUpdated = DateTime.UtcNow;
    }

    private static string? UpdateSocialLinks(string? socialLinkUrl)
    {
        if (string.IsNullOrWhiteSpace(socialLinkUrl))
        {
            return null;
        }

        // If they didn't include https://, add it for them automatically!
        var updatedSocialLinkUrl = socialLinkUrl.StartsWith("http")
            ? socialLinkUrl
            : $"https://{socialLinkUrl}";
        return updatedSocialLinkUrl;
    }
}