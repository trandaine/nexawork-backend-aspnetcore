using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetById;

public record GetCustomerByIdQuery(string Id) : IRequest<CustomerQueryDTO?>;
