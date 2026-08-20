using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Education.Queries.GetById;

public class GetEducationByIdHandler : IRequestHandler<GetEducationByIdQuery, EducationQueryDTO?>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetEducationByIdHandler(
        IEducationRepository educationRepository,
        ICustomerRepository customerRepository
    )
    {
        _educationRepository = educationRepository;
        _customerRepository = customerRepository;
    }

    public async Task<EducationQueryDTO?> Handle(GetEducationByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new Exception("Customer not found");

        var education = await _educationRepository.GetByCustomerIdToEditAsync(customer.CustomerId, cancellationToken);
        if (education == null)
            throw new Exception("Education for this customer does not exist");


        return new EducationQueryDTO(
            education.SchoolName,
            education.Degree,
            education.FieldOfStudy,
            education.StartDate,
            education.EndDate,
            education.Description
        );
    }
}