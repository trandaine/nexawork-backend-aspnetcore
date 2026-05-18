using MediatR;

namespace NexaWork.Application.Features.Client.Skill.Commands.Create;

public record CreateSkillCommand(string Name, string Description) : IRequest<Guid>;