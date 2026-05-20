using System;
using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Commands.Create;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    public CreateCustomerHandler(
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork
    )
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var checkUserExisted = await _customerRepository.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (checkUserExisted is not null)        
            throw new InvalidOperationException("Customer with the given IdentityUserId already exists.");
        var customer = NexaWork.Domain.Entities.Customer.Create(request.IdentityUserId);

        _customerRepository.Create(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.CustomerId;
    }
}

