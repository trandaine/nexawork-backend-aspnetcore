using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Post.Commands.Update;

public class UpdatePostHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostRepository _repository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomerRepository _customerRepository;
    public UpdatePostHandler(
        IPostRepository repository,
        INexaWorkDbContext unitOfWork,
        IFileStorageService fileStorageService,
        ICustomerRepository customerRepository
    )
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _customerRepository = customerRepository;
    }
    public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (customer == null)
        {
            throw new Exception("Customer profile not found for this user.");
        }

        var post = await _repository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            throw new Exception("Post not found.");
        }

        if (post.CustomerId != customer.CustomerId)
        {
            throw new Exception("Unauthorized: You can only update your own posts.");
        }

        string? newMediaUrl = null;
        if (request.MediaFile != null)
        {
            newMediaUrl = await _fileStorageService.UploadFileAsync(request.MediaFile, cancellationToken);

            // Optional Enterprise feature: Delete the old file from the hard drive so your server doesn't run out of space!
            // if (!string.IsNullOrEmpty(post.MediaUrl))
            // {
            //     await _fileStorageService.DeleteFileAsync(post.MediaUrl); 
            // }
        }

        post.Update(
            request.Content,
            newMediaUrl,
            request.Visibility
        );

        _repository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);


    }
}
