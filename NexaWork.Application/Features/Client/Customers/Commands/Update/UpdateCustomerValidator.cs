using FluentValidation;

namespace NexaWork.Application.Features.Client.Customers.Commands.Update;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        // Required customer's LastName or Firstname
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.LastName) || !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("At least one of LastName or FirstName must be provided.");
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
        RuleFor(x => x.Summary)
            .MaximumLength(1000).WithMessage("Summary cannot exceed 1000 characters.");
        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");
        RuleFor(x => x.Headline)
            .MaximumLength(150).WithMessage("Headline cannot exceed 150 characters.");
    }
}