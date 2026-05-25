using FluentValidation;

namespace NexaWork.Application.Features.Client.Education.Commands.Create;

public class CreateEducationValidation :AbstractValidator<CreateEducationCommand>
{
    public CreateEducationValidation()
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
            // .NotEmpty() on a DateTime checks that it isn't "01/01/0001" (the C# default value)
            .NotEmpty().WithMessage("Start date is required.")
            // Prevents users from accidentally entering the year 1800 instead of 1900/2000
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Please enter a valid start date.")
            // Optional: Uncomment the line below if you want to strictly forbid future start dates
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