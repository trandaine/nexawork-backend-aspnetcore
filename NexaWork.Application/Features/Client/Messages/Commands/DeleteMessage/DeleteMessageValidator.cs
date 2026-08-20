using FluentValidation;

namespace NexaWork.Application.Features.Client.Messages.Commands.DeleteMessage;

public class DeleteMessageValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("MessageId không được để trống.");
    }
}
