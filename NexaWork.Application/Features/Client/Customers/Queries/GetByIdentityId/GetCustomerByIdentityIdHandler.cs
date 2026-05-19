using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

public class GetCustomerByIdentityIdHandler : IRequestHandler<GetCustomerByIdentityIdQuery, CustomerWithIdentityIdDTO?>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetCustomerByIdentityIdHandler(ICustomerRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<CustomerWithIdentityIdDTO?> Handle(GetCustomerByIdentityIdQuery request,
        CancellationToken cancellationToken)
    {
        var identityId = _currentUserService.UserId;
        var customerWithIdentityId = await _repository.GetByIdentityIdAsync(identityId, cancellationToken);

        if (customerWithIdentityId == null)
            return null;

        return new CustomerWithIdentityIdDTO(
            // customerWithIdentityId.CustomerId,
            // customerWithIdentityId.IdentityUserId,
            customerWithIdentityId.FirstName,
            customerWithIdentityId.LastName
        );
    }
}