using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Commands.UpdateName;

public class UpdateCustomerNameHandler : IRequestHandler<UpdateCustomerNameCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public UpdateCustomerNameHandler(
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork
    )
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerNameCommand request, CancellationToken cancellationToken)
    {
        var identityUserId = request.UserId;

        var customer = await _customerRepository.GetByIdentityIdToEditAsync(identityUserId, cancellationToken);
        if (customer == null) throw new UnauthorizedAccessException("Customer profile not found.");

        // Call our focused domain method
        customer.UpdateName(request.FirstName, request.LastName);

        // Save exactly what changed
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}