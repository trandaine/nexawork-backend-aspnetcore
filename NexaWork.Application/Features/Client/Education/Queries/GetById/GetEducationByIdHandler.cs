using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Education.Queries.GetById;

public class GetEducationByIdHandler : IRequestHandler<GetEducationByIdQuery, EducationQueryDTO?>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerRepository _customerRepository;

    public GetEducationByIdHandler(
        IEducationRepository educationRepository,
        ICurrentUserService currentUserService,
        ICustomerRepository customerRepository
    )
    {
        _educationRepository = educationRepository;
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
    }

    public async Task<EducationQueryDTO?> Handle(GetEducationByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
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