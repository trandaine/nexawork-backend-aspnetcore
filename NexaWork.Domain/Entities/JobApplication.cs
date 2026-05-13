using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class JobApplication
{
    public Guid JobApplicationId { get; private set; }
    public Guid JobListingId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string ResumeUrl { get; private set; } = string.Empty;
    public string? CoverLetter { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public DateTime AppliedAt { get; private set; } = DateTime.UtcNow;

    public virtual JobListing JobListing { get; private set; } = null!;
    public virtual Customer Customer { get; private set; } = null!;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public void MarkAsDeleted()
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
