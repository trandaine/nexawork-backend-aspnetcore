using FluentValidation;

namespace NexaWork.Application.Features.Client.Education.Commands.Update;

public class UpdateEducationValidator : AbstractValidator<UpdateEducationCommand>
{
    public UpdateEducationValidator()
    {
        RuleFor(x => x.SchoolName)
            .NotEmpty().WithMessage("School name is required.")
            .MaximumLength(150).WithMessage("School name must not exceed 150 characters.");

        RuleFor(x => x.Degree)
            .MaximumLength(100).WithMessage("Degree must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Degree));

        RuleFor(x => x.FieldOfStudy)
            .MaximumLength(100).WithMessage("Field of study must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FieldOfStudy));

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Please enter a valid start date.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date cannot be in the future.");
        
        
        RuleFor(x => x.EndDate)
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Please enter a valid start date.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date cannot be before start date.")
            .When(x => x.EndDate.HasValue);
            

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}