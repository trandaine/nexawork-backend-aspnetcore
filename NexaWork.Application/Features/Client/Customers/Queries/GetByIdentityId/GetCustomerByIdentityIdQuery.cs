using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

public record GetCustomerByIdentityIdQuery() : IRequest<CustomerQueryDTO?>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}
