using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.CustomerAddress.Queries.GetById;

public record GetCustomerAddressByIdQuery() : IRequest<CustomerAddressQueryDTO?>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}