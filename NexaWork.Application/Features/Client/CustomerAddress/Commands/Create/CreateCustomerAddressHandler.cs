using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Create;

public class CreateCustomerAddressHandler : IRequestHandler<CreateCustomerAddressCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public CreateCustomerAddressHandler(
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        ICustomerAddressRepository customerAddressRepository,
        INexaWorkDbContext unitOfWork
        )
    {
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
        _customerAddressRepository = customerAddressRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        // var userIdentityId = _currentUserService.UserId;
        
        var customer = await _customerRepository.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException("Customer not found");
        }
        
        var customerAddressExist = await _customerAddressRepository.GetByCustomerAddressIdAsync(customer.CustomerId, cancellationToken);
        if (customerAddressExist != null)        
            throw new InvalidOperationException("Customer address already exists for this customer");

        var customerAddress = NexaWork.Domain.Entities.CustomerAddress.Create
        (
            customer.CustomerId
        );

        _customerAddressRepository.Create(customerAddress);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customerAddress.CustomerAddressId;
    }
}