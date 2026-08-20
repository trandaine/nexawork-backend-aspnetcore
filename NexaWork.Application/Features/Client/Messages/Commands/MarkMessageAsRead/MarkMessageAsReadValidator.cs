using FluentValidation;

namespace NexaWork.Application.Features.Client.Messages.Commands.MarkMessageAsRead;

public class MarkMessageAsReadValidator : AbstractValidator<MarkMessageAsReadCommand>
{
    public MarkMessageAsReadValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("MessageId không được để trống.");
    }
}
