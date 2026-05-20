using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Update;

public class UpdateCustomerAddressHandler : IRequestHandler<UpdateCustomerAddressCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public UpdateCustomerAddressHandler(
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
    public async Task Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException("Customer not found");
        
        var customerAddress = await _customerAddressRepository.GetByCustomerAddressIdToEditAsync(customer.CustomerId, cancellationToken);
        if (customerAddress == null)
            throw new InvalidOperationException("Customer address not found");
        
        if (customer.CustomerId != customerAddress.CustomerId)
            throw new UnauthorizedAccessException("You do not have permission to edit this address");
        
        customerAddress.Update(
            request.City,
            request.PostalCode,
            request.Country,
            request.TaxId
        );
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}