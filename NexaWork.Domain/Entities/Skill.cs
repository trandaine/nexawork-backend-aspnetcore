using System.Collections.ObjectModel;

namespace NexaWork.Domain.Entities;

public class Skill
{
    public Guid SkillId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<CustomerSkill> CustomerSkills { get; set; } = new Collection<CustomerSkill>();

}
