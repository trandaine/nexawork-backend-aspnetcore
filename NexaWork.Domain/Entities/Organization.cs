using System.Collections.ObjectModel;

namespace NexaWork.Domain.Entities;

public class Organization
{
    
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? OrganizationLogoUrl { get; set; }
    public DateTime? FoundedDate { get; set; }

    public virtual ICollection<JobListing> JobListings { get; set; } = new Collection<JobListing>();

}
