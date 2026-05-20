using FluentValidation;

namespace NexaWork.Application.Features.Client.Comment.Commands.Create;

public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.PostId)
            .NotEqual(Guid.Empty)
            .WithMessage("PostId is required.");


        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Comment content is required.")
            .MaximumLength(1000)
            .WithMessage("Comment content must not exceed 1000 characters.");
    }
}