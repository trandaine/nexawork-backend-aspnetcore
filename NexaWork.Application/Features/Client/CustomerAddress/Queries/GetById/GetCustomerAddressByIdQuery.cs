using MediatR;

namespace NexaWork.Application.Features.Client.CustomerAddress.Queries.GetById;

public record GetCustomerAddressByIdQuery() : IRequest<CustomerAddressQueryDTO?>;