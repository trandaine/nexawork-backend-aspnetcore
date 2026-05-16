using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Commands.Update;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public UpdateCustomerHandler(ICustomerRepository customerRepository, INexaWorkDbContext unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (customer == null || customer.CustomerId != request.CustomerId)
        {
            throw new Exception("Request update customer profile failed");
        }

        customer.Update(
            request.FirstName,
            request.LastName,
            request.Headline,
            request.Summary,
            request.Location
        );

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}