using FluentValidation;
using NexaWork.Application.Features.Client.Reaction.Commands.Create;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Delete;

public class DeleteReactionValidator: AbstractValidator<DeleteReactionCommand>
{
    public DeleteReactionValidator()
    {
        RuleFor(c => c.PostId)
            .Must(id => id != Guid.Empty).WithMessage("Post id is invalid");
    }
}