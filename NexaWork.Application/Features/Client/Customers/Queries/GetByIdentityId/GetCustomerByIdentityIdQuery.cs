using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

public record GetCustomerByIdentityIdQuery(string IdentityUserId) : IRequest<CustomerWithIdentityIdDTO?>;
