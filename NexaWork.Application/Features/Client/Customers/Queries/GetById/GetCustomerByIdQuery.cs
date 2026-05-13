using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerQueryDTO?>;
