using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

public record GetCustomerByIdentityIdQuery() : IRequest<CustomerWithIdentityIdDTO?>;
