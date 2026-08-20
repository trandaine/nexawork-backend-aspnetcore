using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Education.Commands.Create;

public class CreateEducationHandler : IRequestHandler<CreateEducationCommand, Guid>
{
    private readonly IEducationRepository _educationRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public CreateEducationHandler(
        IEducationRepository educationRepository,
        INexaWorkDbContext unitOfWork,
        ICustomerRepository customerRepository
    )
    {
        _educationRepository = educationRepository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }

    public async Task<Guid> Handle(CreateEducationCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new Exception("Customer not found");
        
        var educationExist = await _educationRepository.GetByCustomerIdAsync(customer.CustomerId, cancellationToken);
        if (educationExist != null)
            throw new Exception("Education already exists for this customer");

        var education = Domain.Entities.Education.Create(
            customer.CustomerId,
            request.SchoolName,
            request.Degree,
            request.FieldOfStudy,
            request.StartDate,
            request.EndDate,
            request.Description
        );

        _educationRepository.Create(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return education.EducationId;
    }
}