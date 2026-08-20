using MediatR;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Reaction.Commands.Create;

public class CreateReactionHandler : IRequestHandler<CreateReactionCommand>
{
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly IReactionRepository _reactionRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateReactionHandler(INexaWorkDbContext unitOfWork,
        IReactionRepository reactionRepository, IPostRepository postRepository, ICustomerRepository customerRepository)
    {
        _unitOfWork = unitOfWork;
        _reactionRepository = reactionRepository;
        _customerRepository = customerRepository;
        _postRepository = postRepository;
    }

    public async Task Handle(CreateReactionCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new Exception("Customer not found");

        var post = await _postRepository.GetByIdForEditAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new Exception("Post not found");
        
        bool alreadyReacted = await _reactionRepository.ExistsAsync(customer.CustomerId, post.PostId, cancellationToken);
        if (alreadyReacted)
        {
            return; 
        }

        var reaction = Domain.Entities.Reaction.CreateReacTionForPost(customer.CustomerId, post.PostId);
        _reactionRepository.Add(reaction);
        post.UpdateLikesCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
    }
}