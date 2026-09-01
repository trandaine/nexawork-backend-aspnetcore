using FluentValidation;

namespace NexaWork.Application.Features.Client.Connections.Commands.UnblockConnection;

public class UnblockConnectionValidator : AbstractValidator<UnblockConnectionCommand>
{
    public UnblockConnectionValidator()
    {
        RuleFor(x => x.TargetCustomerId)
            .NotEmpty().WithMessage("Target Customer ID không được để trống.");
    }
}
