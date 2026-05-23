using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;

public class CreateCustomerSocialLinkHandler : IRequestHandler<CreateCustomerSocialLinkCommand, Guid>
{
    private readonly ICustomerSocialLinkRepository _customerSocialLinkRepository;
    private readonly ICustomerRepository _customerRepository;
    // private readonly ICurrentUserService _currentUserService;
    private readonly INexaWorkDbContext _unitOfWork;

    public CreateCustomerSocialLinkHandler(
        ICustomerSocialLinkRepository customerSocialLinkRepository,
        ICustomerRepository customerRepository,
        // ICurrentUserService currentUserService,
        INexaWorkDbContext unitOfWork
    )
    {
        // _currentUserService = currentUserService;
        _customerSocialLinkRepository = customerSocialLinkRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCustomerSocialLinkCommand request, CancellationToken cancellationToken)
    {
        // var userIdentityId = _currentUserService.UserId;

        var customer = await _customerRepository.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (customer == null)
            throw new UnauthorizedAccessException("Customer not found");

        var customerSocialLink =
            NexaWork.Domain.Entities.CustomerSocialLink.Create(
                customer.CustomerId
            );
        _customerSocialLinkRepository.Create(customerSocialLink);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return customerSocialLink.CustomerSocialLinkId;
    }
}