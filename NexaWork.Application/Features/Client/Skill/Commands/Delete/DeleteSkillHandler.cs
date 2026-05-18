using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Skill.Commands.Delete;

public class DeleteSkillHandler : IRequestHandler<DeleteSkillCommand>
{
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ISkillRepository _postRepository;

    public DeleteSkillHandler(INexaWorkDbContext unitOfWork, ISkillRepository postRepository)
    {
        _unitOfWork = unitOfWork;
        _postRepository = postRepository;
    }

    public async Task Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _postRepository.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill == null)
        {
            throw new Exception("Skill not found");
        }

        _postRepository.Remove(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}