using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Skill.Commands.Update;

public class UpdateSkillHandler : IRequestHandler<UpdateSkillCommand>
{
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ISkillRepository _skillRepository;

    public async Task Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _skillRepository.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill == null)
        {
            throw new Exception("Skill not found");
        }

        skill.Update(request.Name, request.Description);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}