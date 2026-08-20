using System;
using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Constants;

namespace NexaWork.Application.Features.Client.Post.Commands.Create;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, Guid>
{
    private readonly IPostRepository _repository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomerRepository _customerRepository;

    public CreatePostHandler(
        
        IPostRepository repository,
        ICustomerRepository customerRepository,
        INexaWorkDbContext unitOfWork,
        IFileStorageService fileStorageService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Guid> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
        {
            throw new UnauthorizedAccessException("Customer profile not found for this user.");
        }

        string? mediaUrl = null;

        // If a file was provided, upload it using the abstracted service
        if (request.MediaFile != null)
        {
            // mediaUrl = await _fileStorageService.UploadFileAsync(request.MediaFile, cancellationToken);
            mediaUrl = await _fileStorageService.UploadFileAsync(request.MediaFile, SubfolderConstants.Posts,
                cancellationToken);
        }


        // Delegate creation to the Domain Entity
        var post = NexaWork.Domain.Entities.Post.Create(
            customer.CustomerId,
            request.Content,
            mediaUrl,
            request.Visibility
        );

        // Track in the repository
        _repository.Add(post);

        // Persist to database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return post.PostId;
    }
}