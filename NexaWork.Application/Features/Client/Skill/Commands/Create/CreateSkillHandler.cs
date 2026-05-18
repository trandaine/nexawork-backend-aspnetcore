using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Skill.Commands.Create;

public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, Guid>
{
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ISkillRepository _postRepository;

    public CreateSkillHandler(INexaWorkDbContext unitOfWork, ISkillRepository postRepository)
    {
        _unitOfWork = unitOfWork;
        _postRepository = postRepository;
    }

    public async Task<Guid> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        var newSkillRecord = NexaWork.Domain.Entities.Skill.Create(request.Name, request.Description);
        
        _postRepository.Add(newSkillRecord);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newSkillRecord.SkillId;
    }
}