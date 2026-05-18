using MediatR;

namespace NexaWork.Application.Features.Client.Skill.Queries.GetAll;

public record GetAllSkillsQuery(): IRequest<List<SkillQueryDTO>>;