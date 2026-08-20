using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetById;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerQueryDTO?>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
