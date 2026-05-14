using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

public class GetCustomerByIdentityIdHandler : IRequestHandler<GetCustomerByIdentityIdQuery, CustomerWithIdentityIdDTO?>
{
    private readonly ICustomerRepository _repository;
    public GetCustomerByIdentityIdHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerWithIdentityIdDTO?> Handle(GetCustomerByIdentityIdQuery request, CancellationToken cancellationToken)
    {
        var customerWithIdentityId = await _repository.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);

        if (customerWithIdentityId == null)
            return null;

        return new CustomerWithIdentityIdDTO(
                    customerWithIdentityId.CustomerId,
                    customerWithIdentityId.IdentityUserId,
                    customerWithIdentityId.FirstName,
                    customerWithIdentityId.LastName
                );




    }

}
