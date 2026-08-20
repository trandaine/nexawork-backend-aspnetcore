using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Queries.GetById;

public record GetCustomerSocialLinkByIdQuery() : IRequest<CustomerSocialLinkQueryDTO?>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}