using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Education.Commands.Update;

public class UpdateEducationHandler : IRequestHandler<UpdateEducationCommand>
{
    private readonly IEducationRepository _educationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public UpdateEducationHandler(
        IEducationRepository educationRepository,
        ICurrentUserService currentUserService,
        INexaWorkDbContext unitOfWork,
        ICustomerRepository customerRepository
    )
    {
        _educationRepository = educationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }
    public async Task Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
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