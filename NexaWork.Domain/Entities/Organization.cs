using System.Collections.ObjectModel;

namespace NexaWork.Domain.Entities;

public class Organization
{
    
    // public Guid OrganizationId { get; set; }
    // public string Name { get; set; } = string.Empty;
    // public string? Industry { get; set; }
    // public string? Location { get; set; }
    // public string? Description { get; set; }
    // public string? WebsiteUrl { get; set; }
    // public string? OrganizationLogoUrl { get; set; }
    // public DateTime? FoundedDate { get; set; }

    // public virtual ICollection<JobListing> JobListings { get; set; } = new Collection<JobListing>();

    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public string? Industry { get; private set; }
    public string? Location { get; private set; }
    public string? Description { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? OrganizationLogoUrl { get; private set; }
    public DateTime? FoundedDate { get; private set; }

    // Navigation
    public virtual ICollection<JobListing> JobListings { get; private set; } = new List<JobListing>();


    // EF Core constructor
    private Organization() { }

    // Business constructor
    public Organization(string name)
    {
        OrganizationId = Guid.NewGuid();
        SetName(name);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required");

        Name = name;
    }

    public void UpdateDetails(
        string? industry,
        string? location,
        string? description,
        string? websiteUrl,
        string? logoUrl,
        DateTime? foundedDate)
    {
        Industry = industry;
        Location = location;
        Description = description;
        WebsiteUrl = websiteUrl;
        OrganizationLogoUrl = logoUrl;
        FoundedDate = foundedDate;
    }
}
