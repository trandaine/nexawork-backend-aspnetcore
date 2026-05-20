using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Comment.Commands.Create;

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICommentRepository _commentRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPostRepository _postRepository;

    public CreateCommentHandler(
        ICurrentUserService currentUserService,
        ICommentRepository commentRepository,
        INexaWorkDbContext unitOfWork,
        ICustomerRepository customerRepository,
        IPostRepository postRepository
    )
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = _currentUserService.UserId;

        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null) 
            throw new UnauthorizedAccessException("Customer profile not found for this user.");

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) 
            throw new Exception("Post not found.");

        var newComment = NexaWork.Domain.Entities.Comment.Create(
            post.PostId,
            customer.CustomerId,
            request.Content
        );

        _commentRepository.Add(newComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newComment.CommentId;
    }
}