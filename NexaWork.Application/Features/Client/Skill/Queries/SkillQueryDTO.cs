namespace NexaWork.Application.Features.Client.Skill.Queries;

public class SkillQueryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}