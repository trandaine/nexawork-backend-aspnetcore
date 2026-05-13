using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Customers.Queries.GetById;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerQueryDTO?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByIdHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }
    public async Task<CustomerQueryDTO?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetCustomerByIdAsync(request.Id, cancellationToken);
        if (customer == null)
            return null;

        return new CustomerQueryDTO(
            customer.CustomerId,
            customer.IdentityUserId,
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
