namespace NexaWork.Application.Features.Client.Education.Queries;

public record EducationQueryDTO(
    string SchoolName,
    string? Degree,
    string? FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description);