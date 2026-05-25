namespace NexaWork.Domain.Entities;

public class Education
{
    public Guid EducationId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string SchoolName { get; private set; } = string.Empty;
    public string? Degree { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Description { get; private set; }

    public virtual Customer Customer { get; private set; } = null!;

    private Education() { }

    public static Education Create(Guid customerId, string schoolName, string? degree, string? fieldOfStudy, DateTime startDate,DateTime? enDate, string? description)
    {
        return new Education
        {
            EducationId = Guid.NewGuid(),
            CustomerId = customerId,
            SchoolName = schoolName,
            Degree = degree,
            FieldOfStudy = fieldOfStudy,
            StartDate = startDate,
            EndDate = enDate,
            Description = description
        };
    }
    
    public void Update(string schoolName, string? degree, string? fieldOfStudy, DateTime startDate, DateTime? endDate, string? description)
    {
        SchoolName = schoolName;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }
}
