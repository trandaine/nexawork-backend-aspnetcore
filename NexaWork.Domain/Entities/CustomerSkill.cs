using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class CustomerSkill
{
    public Guid CustomerSkillId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SkillId { get; set; }
    public ProficiencyLevel ProficiencyLevel { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;
}
