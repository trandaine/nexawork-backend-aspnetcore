using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetAll;

public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerQueryDTO>>
{
    private readonly ICustomerRepository _repository;
    public GetAllCustomersHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<CustomerQueryDTO>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repository.GetAllCustomerAsync(cancellationToken);

        return customers.Select(customer => new CustomerQueryDTO(
            // customer.CustomerId,
            // customer.IdentityUserId,
            customer.FirstName,
            customer.LastName,
            customer.Headline,
            customer.Summary,
            customer.Location,
            customer.ProfilePictureUrl,
            customer.BackgroundPictureUrl
        )).ToList();

    }
}
