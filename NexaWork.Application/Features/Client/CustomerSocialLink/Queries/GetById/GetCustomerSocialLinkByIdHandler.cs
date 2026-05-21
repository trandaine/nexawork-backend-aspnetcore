using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Queries.GetById;

public class
    GetCustomerSocialLinkByIdHandler : IRequestHandler<GetCustomerSocialLinkByIdQuery, CustomerSocialLinkQueryDTO?>
{
    private readonly ICustomerSocialLinkRepository _customerSocialLinkRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCustomerSocialLinkByIdHandler(
        ICustomerSocialLinkRepository customerSocialLinkRepository,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService
    )
    {
        _customerSocialLinkRepository = customerSocialLinkRepository;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CustomerSocialLinkQueryDTO?> Handle(GetCustomerSocialLinkByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;

        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException("Customer not found");

        var customerSocialLink =
            await _customerSocialLinkRepository.GetByCustomerIdAsync(customer.CustomerId, cancellationToken);
        if (customerSocialLink == null)
            throw new InvalidOperationException("Customer social link not found");

        return new CustomerSocialLinkQueryDTO(
            customerSocialLink.FaceBookUrl,
            customerSocialLink.LinkedInUrl,
            customerSocialLink.XUrl,
            customerSocialLink.InstagramUrl
        );
    }
}