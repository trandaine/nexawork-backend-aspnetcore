using FluentValidation;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Create;

public class CreateReactionValidator : AbstractValidator<CreateReactionCommand>
{
    public CreateReactionValidator()
    {
        RuleFor(c => c.PostId)
            .Must(id => id != Guid.Empty).WithMessage("Post id is invalid");
    }
}