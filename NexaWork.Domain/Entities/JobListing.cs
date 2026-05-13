using System.Collections.ObjectModel;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class JobListing
{
    public Guid JobListingId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Requirements { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public EmploymentType EmploymentType { get; private set; }
    public string? SalaryRange { get; private set; }
    public string? ContactEmail { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }



    public virtual Organization Organization { get; private set; } = null!;
    public virtual ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

    public void MarkAsDeleted()
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        // Cascade the soft delete to all active children
        foreach (var application in JobApplications.Where(j => !j.IsDeleted))
        {
            application.MarkAsDeleted();
        }

    }
}
