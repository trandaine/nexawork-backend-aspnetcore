using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class JobApplication
{
    public Guid JobApplicationId { get; set; }
    public Guid JobListingId { get; set; }
    public Guid CustomerId { get; set; }
    public string ResumeUrl { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public virtual JobListing JobListing { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
}
