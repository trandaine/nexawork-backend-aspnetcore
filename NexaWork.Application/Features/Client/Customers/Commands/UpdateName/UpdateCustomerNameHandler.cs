using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Customers.Commands.UpdateName;

public class UpdateCustomerNameHandler : IRequestHandler<UpdateCustomerNameCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCustomerNameHandler(
        ICustomerRepository customerRepository,
        INexaWorkDbContext context,
        ICurrentUserService currentUserService
        )
    {
        _customerRepository = customerRepository;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateCustomerNameCommand request, CancellationToken cancellationToken)
    {
        var identityUserId = _currentUserService.UserId;
        var customer = await _customerRepository.GetByIdentityIdToEditAsync(identityUserId, cancellationToken);

        if (customer == null) throw new UnauthorizedAccessException("Customer profile not found.");

        // Call our focused domain method
        customer.UpdateName(request.FirstName, request.LastName);
        
        // Save exactly what changed
        await _context.SaveChangesAsync(cancellationToken);
    }
}