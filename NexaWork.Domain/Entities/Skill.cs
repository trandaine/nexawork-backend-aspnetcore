using System.Collections.ObjectModel;

namespace NexaWork.Domain.Entities;

public class Skill
{
    public Guid SkillId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public virtual ICollection<CustomerSkill> CustomerSkills { get; private set; } = new Collection<CustomerSkill>();

    
    private Skill()
    {
        // Required by EF Core
    }
    
    public static Skill Create(string name, string description)
    {
        // bool hasData = !String.IsNullOrEmpty(name);
        if (!CheckEmptyName(name))
            throw new ArgumentException("Skill must have a name.");

        return new Skill
        {
            SkillId = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }

    public void Update(string name, string description)
    {
        if (!CheckEmptyName(name))
            throw new ArgumentException("Skill must have a name.");
        
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Method to check that if the input is empty
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool CheckEmptyName(string name)
    {
        bool hasData = !String.IsNullOrEmpty(name);
        return hasData;
    }

}
