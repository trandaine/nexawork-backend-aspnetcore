using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Queries.GetById;

public class
    GetCustomerSocialLinkByIdHandler : IRequestHandler<GetCustomerSocialLinkByIdQuery, CustomerSocialLinkQueryDTO?>
{
    private readonly ICustomerSocialLinkRepository _customerSocialLinkRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerSocialLinkByIdHandler(
        ICustomerSocialLinkRepository customerSocialLinkRepository,
        ICustomerRepository customerRepository
    )
    {
        _customerSocialLinkRepository = customerSocialLinkRepository;
        _customerRepository = customerRepository;
    }

    public async Task<CustomerSocialLinkQueryDTO?> Handle(GetCustomerSocialLinkByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;

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