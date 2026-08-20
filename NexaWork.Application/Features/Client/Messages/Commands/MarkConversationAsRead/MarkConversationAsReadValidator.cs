using FluentValidation;

namespace NexaWork.Application.Features.Client.Messages.Commands.MarkConversationAsRead;

public class MarkConversationAsReadValidator : AbstractValidator<MarkConversationAsReadCommand>
{
    public MarkConversationAsReadValidator()
    {
        RuleFor(x => x.OtherCustomerId)
            .NotEmpty().WithMessage("OtherCustomerId không được để trống.");
    }
}
