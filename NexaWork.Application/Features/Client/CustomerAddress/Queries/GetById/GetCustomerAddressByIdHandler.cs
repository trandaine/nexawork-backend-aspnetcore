using System.Security.Authentication;
using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.CustomerAddress.Queries.GetById;

public class GetCustomerAddressByIdHandler : IRequestHandler<GetCustomerAddressByIdQuery, CustomerAddressQueryDTO?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerAddressRepository _customerAddressRepository;

    public GetCustomerAddressByIdHandler(
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        ICustomerAddressRepository customerAddressRepository
    )
    {
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
        _customerAddressRepository = customerAddressRepository;
    }
    public async Task<CustomerAddressQueryDTO?> Handle(GetCustomerAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
        
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