using FluentValidation;
using PhoneNumbers;

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
        
        // Validate phonenumber
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(16).WithMessage("Phone number cannot exceed 16 characters.")
            .Must(BeAValidPhoneNumber).WithMessage("The phone number provided is not a valid, routable number.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        
        
        When(v => v.BackgroundPictureFile != null, () =>
        {
            RuleFor(v => v.BackgroundPictureFile!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024) // 5 MB limit
                .WithMessage("The media file must not exceed 5 MB.");

            RuleFor(v => v.BackgroundPictureFile!.ContentType)
                .Must(BeAValidImage)
                .WithMessage("Only JPEG and PNG images are allowed.");
        });

        When(v => v.ProfilePictureFile != null, () =>
        {
            RuleFor(v => v.ProfilePictureFile!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024) // 5 MB limit
                .WithMessage("The media file must not exceed 5 MB.");

            RuleFor(v => v.ProfilePictureFile!.ContentType)
                .Must(BeAValidImage)
                .WithMessage("Only JPEG and PNG images are allowed.");
        });
    }

    private bool BeAValidImage(string contentType)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        return allowedTypes.Contains(contentType.ToLower());
    }
    
    private bool BeAValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        try
        {
            // The "ZZ" means we expect the user to include the country code (like +84 or +1)
            // If you only operate in Vietnam, you could change "ZZ" to "VN" and it will assume local numbers!
            var parsedNumber = phoneNumberUtil.Parse(phoneNumber, "VN");
            
            return phoneNumberUtil.IsValidNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            // If it can't even be parsed, it's definitely invalid
            return false;
        }
    }
}