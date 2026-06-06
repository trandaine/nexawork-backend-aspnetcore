using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Delete;

public class DeleteReactionHandler : IRequestHandler<DeleteReactionCommand>
{
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IReactionRepository _reactionRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICustomerRepository _customerRepository;

    public DeleteReactionHandler(INexaWorkDbContext unitOfWork, ICurrentUserService currentUserService,
        IReactionRepository reactionRepository, IPostRepository postRepository, ICustomerRepository customerRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _reactionRepository = reactionRepository;
        _customerRepository = customerRepository;
        _postRepository = postRepository;
    }
    public async Task Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new Exception("Customer not found");

        var post = await _postRepository.GetByIdForEditAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new Exception("Post not found");
        
        var reaction = await _reactionRepository.GetByCustomerIdAndPostIdAsync(customer.CustomerId, post.PostId, cancellationToken);
        if (reaction == null)
        {
            return; 
        }

        _unitOfWork.Reactions.Remove(reaction);
        post.DecrementLikesCount();
        _postRepository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}