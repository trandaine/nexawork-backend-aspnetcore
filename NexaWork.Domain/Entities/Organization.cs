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
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public bool IsValidated { get; private set; }
    public DateTime? ValidatedAt { get; private set; }


    // Navigation
    public virtual ICollection<JobListing> JobListings { get; private set; } = new List<JobListing>();


    // EF Core constructor
    private Organization() { }

    // Business constructor


    /// <summary>
    /// Creates a new organization instance. 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="industry"></param>
    /// <param name="location"></param>
    /// <param name="description"></param>
    /// <param name="websiteUrl"></param>
    /// <param name="logoUrl"></param>
    /// <param name="foundedDate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Organization Create(
        string name, string? industry, string? location,
        string? description, string? websiteUrl,
        string? logoUrl, DateTime? foundedDate
        )
    {
        // Example of domain logic/validation
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name cannot be empty.");

        return new Organization
        {
            OrganizationId = Guid.NewGuid(),
            Name = name,
            Industry = industry,
            Location = location,
            Description = description,
            WebsiteUrl = websiteUrl,
            OrganizationLogoUrl = logoUrl,
            FoundedDate = foundedDate,
            IsDeleted = false,
            IsValidated = false
        };
    }

    public void Update(
    string name,
    string? industry,
    string? location,
    string? description,
    string? websiteUrl,
    string? logoUrl,
    DateTime? foundedDate)
    {
        Name = name;
        Industry = industry;
        Location = location;
        Description = description;
        WebsiteUrl = websiteUrl;
        OrganizationLogoUrl = logoUrl;
        FoundedDate = foundedDate;

    }

    /// <summary>
    /// Marks the organization as validated. This operation is only done by administrators
    /// </summary>
    public void MarkAsValidated()
    {
        // if (IsValidated)
        // {
        //     throw new InvalidOperationException("Organization is already validated.");
        // }
        if (IsValidated) return;

        IsValidated = true;
        ValidatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revokes the validation of the organization. This operation is only done by administrators
    /// </summary>
    public void RevokeValidation()
    {
        if (!IsValidated) return;

        IsValidated = false;
        ValidatedAt = null;
    }



    /// <summary>
    /// Marks the organization as deleted. This operation is only done by administrators
    /// </summary>
    public void MarkAsDeleted()
    {
        // if (IsDeleted)
        // {
        //     throw new InvalidOperationException("Organization is already marked as deleted.");
        // }
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        // Cascade the soft delete to all active children
        foreach (var job in JobListings.Where(j => !j.IsDeleted))
        {
            job.MarkAsDeleted();
        }
    }

    /// <summary>
    /// Restores the organization from a deleted state. This operation is only done by administrators
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
    }




}
