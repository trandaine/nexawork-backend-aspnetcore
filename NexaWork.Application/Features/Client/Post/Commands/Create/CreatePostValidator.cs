using System;
using FluentValidation;

namespace NexaWork.Application.Features.Client.Post.Commands.Create;

public class CreatePostValidator : AbstractValidator<CreatePostCommand>
{
    private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".mp4" };

    public CreatePostValidator()
    {
        RuleFor(p => p.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(p => p.Content)
            .MaximumLength(3000).WithMessage("Post content cannot exceed 3000 characters.");

        RuleFor(p => p.Visibility)
            .IsInEnum().WithMessage("Invalid visibility level.");

        // Validator enforcing the same rule as the domain: must have text OR media
        RuleFor(p => p)
            .Must(p => !string.IsNullOrWhiteSpace(p.Content) || p.MediaFile != null)
            .WithMessage("A post must contain either text or a media file.");


        When(v => v.MediaFile != null, () =>
        {
            RuleFor(v => v.MediaFile!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024) // 5 MB limit
                .WithMessage("The media file must not exceed 5 MB.");

            RuleFor(v => v.MediaFile!.ContentType)
                .Must(BeAValidImage)
                .WithMessage("Only JPEG and PNG images are allowed.");
        });
    }

    private bool BeAValidImage(string contentType)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        return allowedTypes.Contains(contentType.ToLower());
    }
}