using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Skill.Queries.GetById;

public class GetSkillByIdHandler : IRequestHandler<GetSkillByIdQuery, SkillQueryDTO>
{
    private readonly ISkillRepository _repository;

    public GetSkillByIdHandler(ISkillRepository repository)
    {
        _repository = repository;
    }

    public async Task<SkillQueryDTO> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
    {
        var skill = await _repository.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill == null)
            throw new KeyNotFoundException($"Skill with ID {request.SkillId} not found.");
        
        return new SkillQueryDTO
        {
            Id = skill.SkillId,
            Name = skill.Name,
            Description = skill.Description
        };
    }
}