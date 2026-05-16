using FluentValidation;

namespace NexaWork.Application.Features.Client.Comment.Commands.Delete;

public class DeleteCommentValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentValidator()
    {
        RuleFor(c => c.CommentId)
            .NotEqual(Guid.Empty)
            .WithMessage("CommentId is required.");
    }
}