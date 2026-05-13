using FluentValidation;

namespace NexaWork.Application.Features.Client.Organization.Commands.Delete;

public class DeleteOrganizationValidator : AbstractValidator<DeleteOrganizationCommand>
{
    public DeleteOrganizationValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required for deletion.");
    }
}
