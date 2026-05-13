using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetAll;

public record GetAllCustomersQuery() : IRequest<List<CustomerQueryDTO>>;
