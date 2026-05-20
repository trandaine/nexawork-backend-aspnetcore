using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Constants;

namespace NexaWork.Application.Features.Client.Customers.Commands.Update;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;


    public UpdateCustomerHandler(
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService
    )
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var identityUserId = _currentUserService.UserId;
        var customer = await _customerRepository.GetByIdentityIdToEditAsync(identityUserId, cancellationToken);
        if (customer == null)
        {
            throw new UnauthorizedAccessException("Request update customer profile failed");
        }

        string? profilePictureUrl = null;
        string? backgroundPictureUrl = null;

        if (request.ProfilePictureFile != null)
        {
            profilePictureUrl = await _fileStorageService.UploadFileAsync(request.ProfilePictureFile,
                SubfolderConstants.CustomerProfilePictures,
                cancellationToken);
        }

        if (request.BackgroundPictureFile != null)
        {
            backgroundPictureUrl = await _fileStorageService.UploadFileAsync(request.BackgroundPictureFile,
                SubfolderConstants.CustomerBackGroundPictures,
                cancellationToken);
        }

        customer.Update(
            request.FirstName,
            request.LastName,
            request.Headline,
            request.Summary,
            request.Location,
            profilePictureUrl,
            backgroundPictureUrl,
            request.PhoneNumber
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}