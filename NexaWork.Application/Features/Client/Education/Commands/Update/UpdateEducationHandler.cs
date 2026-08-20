using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Education.Commands.Update;

public class UpdateEducationHandler : IRequestHandler<UpdateEducationCommand>
{
    private readonly IEducationRepository _educationRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public UpdateEducationHandler(
        IEducationRepository educationRepository,
        INexaWorkDbContext unitOfWork,
        ICustomerRepository customerRepository
    )
    {
        _educationRepository = educationRepository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }
    public async Task Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new Exception("Customer not found");
        
        var education = await _educationRepository.GetByCustomerIdToEditAsync(customer.CustomerId, cancellationToken);
        if (education == null)
            throw new Exception("Education not found");
        
        education.Update(
            request.SchoolName,
            request.Degree,
            request.FieldOfStudy,
            request.StartDate,
            request.EndDate,
            request.Description
        );
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}