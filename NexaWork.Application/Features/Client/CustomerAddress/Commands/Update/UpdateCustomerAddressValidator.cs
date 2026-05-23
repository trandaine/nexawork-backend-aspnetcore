using FluentValidation;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Update;

public class UpdateCustomerAddressValidator : AbstractValidator<UpdateCustomerAddressCommand>
{
    public UpdateCustomerAddressValidator()
    {
        RuleFor(cav => cav.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
            .Matches(@"^[\p{L}\s\-\']+$")
            .WithMessage("City cannot contain numbers or special characters.")
            .When(cav => !string.IsNullOrWhiteSpace(cav.City));
        RuleFor(cav => cav.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.");
        RuleFor(cav => cav.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
            .Matches(@"^[\p{L}\s\-\']+$")
            .WithMessage("Country cannot contain numbers or special characters.")
            .When(cav => !string.IsNullOrWhiteSpace(cav.Country));
        RuleFor(cav => cav.TaxId)
            .MaximumLength(50).WithMessage("Tax ID must not exceed 50 characters.");
    }
}