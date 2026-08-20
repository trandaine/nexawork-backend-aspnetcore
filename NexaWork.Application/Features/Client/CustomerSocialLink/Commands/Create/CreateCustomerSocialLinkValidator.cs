using FluentValidation;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;

public class CreateCustomerSocialLinkValidator : AbstractValidator<CreateCustomerSocialLinkCommand>
{
    public CreateCustomerSocialLinkValidator()
    {
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("UserId must be a valid GUID.");
        // RuleFor(slv => slv.FaceBookUrl)
        //     .MaximumLength(200).WithMessage("Facebook URL must not exceed 200 characters.");
        //     
        // RuleFor(slv => slv.LinkedInUrl)
        //     .MaximumLength(200).WithMessage("LinkedIn URL must not exceed 200 characters.");
        // RuleFor(slv => slv.XUrl)
        //     .MaximumLength(200).WithMessage("Twitter URL must not exceed 200 characters.");
        // RuleFor(slv => slv.InstagramUrl)
        //     .MaximumLength(200).WithMessage("Instagram URL must not exceed 200 characters.");

        // 1. Facebook
        // RuleFor(x => x.FaceBookUrl)
        //     .MaximumLength(200).WithMessage("Facebook URL must not exceed 200 characters.")
        //     .Matches(@"^(https?:\/\/)?(www\.)?facebook\.com\/[a-zA-Z0-9\.]+")
        //     .WithMessage("Must be a valid Facebook profile URL (e.g., facebook.com/username).") // 👈 Moved up!
        //     .When(x => !string.IsNullOrWhiteSpace(x.FaceBookUrl)); // 👈 When applies to the whole chain
        //
        // // 2. LinkedIn
        // RuleFor(x => x.LinkedInUrl)
        //     .MaximumLength(200).WithMessage("LinkedIn URL must not exceed 200 characters.")
        //     .Matches(@"^(https?:\/\/)?(www\.)?linkedin\.com\/in\/[a-zA-Z0-9_-]+")
        //     .WithMessage("Must be a valid LinkedIn profile URL (e.g., linkedin.com/in/username).")
        //     .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl));
        //
        // // 3. Twitter / X
        // RuleFor(x => x.XUrl)
        //     .MaximumLength(200).WithMessage("Twitter/X URL must not exceed 200 characters.")
        //     .Matches(@"^(https?:\/\/)?(www\.)?(twitter\.com|x\.com)\/[a-zA-Z0-9_]+")
        //     .WithMessage("Must be a valid X or Twitter URL (e.g., x.com/username).")
        //     .When(x => !string.IsNullOrWhiteSpace(x.XUrl));
        //
        // // 4. Instagram
        // RuleFor(x => x.InstagramUrl)
        //     .MaximumLength(200).WithMessage("Instagram URL must not exceed 200 characters.")
        //     .Matches(@"^(https?:\/\/)?(www\.)?instagram\.com\/[a-zA-Z0-9_\.]+")
        //     .WithMessage("Must be a valid Instagram profile URL (e.g., instagram.com/username).")
        //     .When(x => !string.IsNullOrWhiteSpace(x.InstagramUrl));

        // 4. GitHub Validation
        // RuleFor(x => x.GithubUrl)
        //     .Matches(@"^(https?:\/\/)?(www\.)?github\.com\/[a-zA-Z0-9_-]+")
        //     .When(x => !string.IsNullOrWhiteSpace(x.GithubUrl))
        //     .WithMessage("Must be a valid GitHub profile URL (e.g., github.com/username).");
    }
}