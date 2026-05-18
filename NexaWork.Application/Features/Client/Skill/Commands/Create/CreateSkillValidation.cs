using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace NexaWork.Application.Features.Client.Skill.Commands.Create;

public class CreateSkillValidation : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");
    }
}

