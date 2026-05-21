using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Update;

public class UpdateCustomerSocialLinkHandler : IRequestHandler<UpdateCustomerSocialLinkCommand>
{
    private readonly ICustomerSocialLinkRepository _customerSocialLinkRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INexaWorkDbContext _unitOfWork;

    public UpdateCustomerSocialLinkHandler(
        ICustomerSocialLinkRepository customerSocialLinkRepository,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        INexaWorkDbContext unitOfWork
    )
    {
        _currentUserService = currentUserService;
        _customerSocialLinkRepository = customerSocialLinkRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
        
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new UnauthorizedAccessException("Customer not found");
        
        var customerSocialLinks = await _customerSocialLinkRepository.GetByCustomerIdToEditAsync(customer.CustomerId, cancellationToken);
        if (customerSocialLinks == null)
            throw new InvalidOperationException("Customer social links not found");
        
        if(customer.CustomerId != customerSocialLinks.CustomerId)
            throw new UnauthorizedAccessException("Request update customer social links failed");
        
        customerSocialLinks.Update(
            request.FaceBookUrl,
            request.LinkedInUrl,
            request.XUrl,
            request.InstagramUrl
        );
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}