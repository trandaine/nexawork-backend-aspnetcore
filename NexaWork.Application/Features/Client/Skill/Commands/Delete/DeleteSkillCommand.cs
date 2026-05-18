using MediatR;

namespace NexaWork.Application.Features.Client.Skill.Commands.Delete;

public record DeleteSkillCommand(Guid SkillId) : IRequest;