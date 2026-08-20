using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

public class GetCustomerByIdentityIdHandler : IRequestHandler<GetCustomerByIdentityIdQuery, CustomerQueryDTO?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByIdentityIdHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerQueryDTO?> Handle(GetCustomerByIdentityIdQuery request,
        CancellationToken cancellationToken)
    {
        var identityId = request.UserId;
        var customerWithIdentityId = await _repository.GetByIdentityIdAsync(identityId, cancellationToken);

        if (customerWithIdentityId == null)
            return null;

        return new CustomerQueryDTO(
            // // customerWithIdentityId.CustomerId,
            // // customerWithIdentityId.IdentityUserId,
            // customerWithIdentityId.FirstName,
            // customerWithIdentityId.LastName
            customerWithIdentityId.CustomerId,
            customerWithIdentityId.FirstName,
            customerWithIdentityId.LastName,
            customerWithIdentityId.Headline,
            customerWithIdentityId.Summary,
            customerWithIdentityId.Location,
            customerWithIdentityId.ProfilePictureUrl,
            customerWithIdentityId.BackgroundPictureUrl,
            customerWithIdentityId.PhoneNumber
        );
    }
}