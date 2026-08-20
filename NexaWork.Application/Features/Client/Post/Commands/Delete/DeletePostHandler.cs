using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Post.Commands.Delete;

public class DeletePostHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public DeletePostHandler(
        IPostRepository postRepository,
        INexaWorkDbContext unitOfWork,
        ICustomerRepository customerRepository
    )
    {
        _postRepository = postRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;

        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new UnauthorizedAccessException("Customer profile not found for this user.");

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new Exception($"Post with ID {request.PostId} not found");
        
        if(post.CustomerId != customer.CustomerId)
            throw new UnauthorizedAccessException("You can only delete your own posts.");
        
        _postRepository.Remove(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}