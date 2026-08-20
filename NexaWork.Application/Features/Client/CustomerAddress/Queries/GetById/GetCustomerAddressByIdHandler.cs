using System.Security.Authentication;
using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.CustomerAddress.Queries.GetById;

public class GetCustomerAddressByIdHandler : IRequestHandler<GetCustomerAddressByIdQuery, CustomerAddressQueryDTO?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerAddressRepository _customerAddressRepository;

    public GetCustomerAddressByIdHandler(
        ICustomerRepository customerRepository,
        ICustomerAddressRepository customerAddressRepository
    )
    {
        _customerRepository = customerRepository;
        _customerAddressRepository = customerAddressRepository;
    }
    public async Task<CustomerAddressQueryDTO?> Handle(GetCustomerAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if(customer == null)
            throw new AuthenticationException("Customer not found");
        
        var customerAddress = await _customerAddressRepository.GetByCustomerAddressIdAsync(customer.CustomerId, cancellationToken);
        if(customerAddress == null)
            throw new InvalidOperationException("Customer address not found");  
        
        return new CustomerAddressQueryDTO(
            customerAddress.City,
            customerAddress.PostalCode,
            customerAddress.Country,
            customerAddress.TaxId
        );
    }
}