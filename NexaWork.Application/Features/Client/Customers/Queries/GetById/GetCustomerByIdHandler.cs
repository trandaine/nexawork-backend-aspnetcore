using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetById;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerQueryDTO?>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetCustomerByIdHandler(ICustomerRepository repository, ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        _repository = repository;
    }
    public async Task<CustomerQueryDTO?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var identityId = _currentUserService.UserId;
        // var customer = await _repository.GetCustomerByIdAsync(request.Id, cancellationToken);
        var customer = await _repository.GetByIdentityIdAsync(identityId, cancellationToken);
        if (customer == null)
            return null;

        return new CustomerQueryDTO(
            // customer.CustomerId,
            // customer.IdentityUserId,
            customer.FirstName,
            customer.LastName,
            customer.Headline,
            customer.Summary,
            customer.Location,
            customer.ProfilePictureUrl,
            customer.BackgroundPictureUrl
        );
    }
}
