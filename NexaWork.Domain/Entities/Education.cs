namespace NexaWork.Domain.Entities;

public class Education
{
    public Guid EducationId { get; set; }
    public Guid CustomerId { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string? Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
