using FluentValidation;

namespace NexaWork.Application.Features.Client.Messages.Commands.SendMessage;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.ReceiverCustomerId)
            .NotEmpty().WithMessage("Người nhận không được để trống.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung tin nhắn không được để trống.")
            .MaximumLength(2000).WithMessage("Nội dung tin nhắn không được vượt quá 2000 ký tự.");
    }
}
