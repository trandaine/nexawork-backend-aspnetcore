using FluentValidation;

namespace NexaWork.Application.Features.Client.Customers.Commands.UpdateName;

public class UpdateCustomerNameValidator : AbstractValidator<UpdateCustomerNameCommand>
{
    public UpdateCustomerNameValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.LastName) || !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("At least one of LastName or FirstName must be provided.");
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
    }
}