using System;
using FluentValidation;

namespace NexaWork.Application.Features.Client.Customers.Commands.Create;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.IdentityUserId)
            .NotEmpty().WithMessage("IdentityUserId is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("IdentityUserId must be a valid GUID.");
    }
}
