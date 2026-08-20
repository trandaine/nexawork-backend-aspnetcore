using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Constants;

namespace NexaWork.Application.Features.Client.Post.Commands.Update;

public class UpdatePostHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostRepository _postRepository;
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
        _postRepository = repository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _customerRepository = customerRepository;
    }

    public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var identityUserId = request.UserId;

        var customer = await _customerRepository.GetByIdentityIdAsync(identityUserId, cancellationToken);
        if (customer == null)
            throw new UnauthorizedAccessException("Customer profile not found for this user.");

        var post = await _postRepository.GetByIdForEditAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new Exception("Post not found.");

        if (post.CustomerId != customer.CustomerId)
            throw new UnauthorizedAccessException("You can only update your own posts.");

        string? newMediaUrl = null;
        if (request.MediaFile != null)
        {
            // newMediaUrl = await _fileStorageService.UploadFileAsync(request.MediaFile, cancellationToken);
            newMediaUrl = await _fileStorageService.UploadFileAsync(request.MediaFile, SubfolderConstants.Posts,
                cancellationToken);

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

        // Disable this because of it will override the record instead of update only the Content, MediaURL and visibility
        // _repository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}