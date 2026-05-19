using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Commands.UpdateName;

public class UpdateCustomerNameHandler : IRequestHandler<UpdateCustomerNameCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _context;

    public UpdateCustomerNameHandler(
        ICustomerRepository customerRepository,
        INexaWorkDbContext context)
    {
        _customerRepository = customerRepository;
        _context = context;
    }

    public async Task Handle(UpdateCustomerNameCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdentityIdToEditAsync(request.IdentityUserId, cancellationToken);

        if (customer == null) throw new UnauthorizedAccessException("Customer profile not found.");

        // Call our focused domain method
        customer.UpdateName(request.FirstName, request.LastName);
        
        // Save exactly what changed
        await _context.SaveChangesAsync(cancellationToken);
    }
}