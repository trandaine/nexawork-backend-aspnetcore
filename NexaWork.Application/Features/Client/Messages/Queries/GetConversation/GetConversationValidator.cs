using FluentValidation;

namespace NexaWork.Application.Features.Client.Messages.Queries.GetConversation;

public class GetConversationValidator : AbstractValidator<GetConversationQuery>
{
    public GetConversationValidator()
    {
        RuleFor(x => x.OtherCustomerId)
            .NotEmpty().WithMessage("OtherCustomerId không được để trống.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page phải lớn hơn 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("PageSize phải từ 1 đến 50.");
    }
}
