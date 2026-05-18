using MediatR;

namespace NexaWork.Application.Features.Client.Skill.Queries.GetById;

public record GetSkillByIdQuery(Guid SkillId) : IRequest<SkillQueryDTO>;