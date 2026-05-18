using MediatR;

namespace NexaWork.Application.Features.Client.Skill.Commands.Update;

public record UpdateSkillCommand(Guid SkillId, string Name, string Description) : IRequest;
