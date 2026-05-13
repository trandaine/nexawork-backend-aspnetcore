using System;
using FluentValidation;

namespace NexaWork.Application.Features.Client.Organization.Commands.Update;

public class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Organization name is required.")
            .MaximumLength(200).WithMessage("Organization name must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Organization description is required.")
            .MaximumLength(1000).WithMessage("Organization description must not exceed 1000 characters.");


        RuleFor(v => v.WebsiteUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Website URL must be a valid format.");

        RuleFor(v => v.FoundedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(v => v.FoundedDate.HasValue)
            .WithMessage("Founded date cannot be in the future.");
    }

}
