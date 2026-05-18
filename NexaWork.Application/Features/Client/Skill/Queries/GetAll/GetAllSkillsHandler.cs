using MediatR;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Skill.Queries.GetAll;

public class GetAllSkillsHandler : IRequestHandler<GetAllSkillsQuery, List<SkillQueryDTO>>
{
    private readonly INexaWorkDbContext _unitOfWork;

    public GetAllSkillsHandler(INexaWorkDbContext unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<SkillQueryDTO>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Skills
            .AsNoTracking()
            .Select(s => new SkillQueryDTO
            {
                Id = s.SkillId,
                Name = s.Name,
                Description = s.Description
            }).ToListAsync(cancellationToken);;
    }
}