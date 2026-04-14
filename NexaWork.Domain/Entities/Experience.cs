using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Experience
{
    public Guid ExperienceId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsCurrent { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual Organization? Organization { get; set; }
}
