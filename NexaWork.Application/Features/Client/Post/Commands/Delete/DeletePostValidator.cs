using System;
using FluentValidation;

namespace NexaWork.Application.Features.Client.Post.Commands.Delete;

public class DeletePostValidator: AbstractValidator<DeletePostCommand>
{
    public DeletePostValidator()
    {
        RuleFor(p => p.PostId)
            .NotEmpty().WithMessage("Post ID is required for deletion.");
    }
}
