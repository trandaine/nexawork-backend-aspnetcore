using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Reaction.Queries.GetPostComment;

public class GetCustomerReactionByPostIdHandler : IRequestHandler<GetCustomerReactionByPostIdQuery, Boolean>
{
    private readonly IReactionRepository _reactionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPostRepository _postRepository;

    public GetCustomerReactionByPostIdHandler(IReactionRepository reactionRepository,
        ICurrentUserService currentUserService, ICustomerRepository customerRepository, IPostRepository postRepository)
    {
        _reactionRepository = reactionRepository;
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
        _postRepository = postRepository;
    }

    public async Task<bool> Handle(GetCustomerReactionByPostIdQuery request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;
        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new Exception("Customer not found");

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new Exception("Post not found");
        
        
        return await _reactionRepository.ExistsAsync(customer.CustomerId, request.PostId, cancellationToken);
    }
}