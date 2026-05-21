using FluentValidation;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Update;

public class UpdateCustomerSocialLinkValidator : AbstractValidator<UpdateCustomerSocialLinkCommand>
{
    public UpdateCustomerSocialLinkValidator()
    {
        // 1. Facebook
        RuleFor(x => x.FaceBookUrl)
            .MaximumLength(200).WithMessage("Facebook URL must not exceed 200 characters.")
            .Matches(@"^(https?:\/\/)?(www\.)?facebook\.com\/[a-zA-Z0-9\.]+")
            .WithMessage("Must be a valid Facebook profile URL (e.g., facebook.com/username).") // 👈 Moved up!
            .When(x => !string.IsNullOrWhiteSpace(x.FaceBookUrl)); // 👈 When applies to the whole chain

// 2. LinkedIn
        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(200).WithMessage("LinkedIn URL must not exceed 200 characters.")
            .Matches(@"^(https?:\/\/)?(www\.)?linkedin\.com\/in\/[a-zA-Z0-9_-]+")
            .WithMessage("Must be a valid LinkedIn profile URL (e.g., linkedin.com/in/username).")
            .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl));

// 3. Twitter / X
        RuleFor(x => x.XUrl)
            .MaximumLength(200).WithMessage("Twitter/X URL must not exceed 200 characters.")
            .Matches(@"^(https?:\/\/)?(www\.)?(twitter\.com|x\.com)\/[a-zA-Z0-9_]+")
            .WithMessage("Must be a valid X or Twitter URL (e.g., x.com/username).")
            .When(x => !string.IsNullOrWhiteSpace(x.XUrl));

// 4. Instagram
        RuleFor(x => x.InstagramUrl)
            .MaximumLength(200).WithMessage("Instagram URL must not exceed 200 characters.")
            .Matches(@"^(https?:\/\/)?(www\.)?instagram\.com\/[a-zA-Z0-9_\.]+")
            .WithMessage("Must be a valid Instagram profile URL (e.g., instagram.com/username).")
            .When(x => !string.IsNullOrWhiteSpace(x.InstagramUrl));
    }
}