using FluentValidation;

namespace NexaWork.Application.Features.Client.Comment.Commands.Update;

public class UpdateCommentValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000).WithMessage("Comment content must not exceed 1000 characters.");
        
        RuleFor(comment => comment.CommentId)
            .NotEqual(Guid.Empty).WithMessage("CommentId to update is required.");
    }
}