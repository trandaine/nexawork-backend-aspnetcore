using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly NexaWorkDbContext _context;

    public SkillRepository(NexaWorkDbContext context)
    {
        _context = context;
    }

    public void Add(Skill skill)
    {
        _context.Skills.Add(skill);
    }

    public async Task<List<Skill>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Skills
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Skills
            // .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SkillId == id, cancellationToken);
    }

    public void Update(Skill skill)
    {
        _context.Skills.Update(skill);
    }

    public void Remove(Skill skill)
    {
        _context.Skills.Remove(skill);
    }
}