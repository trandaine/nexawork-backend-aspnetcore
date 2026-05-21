using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Queries.GetById;

public record GetCustomerSocialLinkByIdQuery() : IRequest<CustomerSocialLinkQueryDTO?>;