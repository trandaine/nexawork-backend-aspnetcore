using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Comment.Commands.Update;

public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly IPostRepository _postRepository;
    private readonly ICustomerRepository _customerRepository;

    public UpdateCommentHandler(
        ICommentRepository commentRepository,
        INexaWorkDbContext unitOfWork,
        IPostRepository postRepository,
        ICustomerRepository customerRepository
    )
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _postRepository = postRepository;
        _customerRepository = customerRepository;
    }

    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdentityId = request.UserId;

        var customer = await _customerRepository.GetByIdentityIdAsync(userIdentityId, cancellationToken);
        if (customer == null)
            throw new UnauthorizedAccessException("Unauthorized");

        var updateComment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (updateComment == null)
            throw new InvalidOperationException("Comment not found.");

        if (updateComment.CustomerId != customer.CustomerId)
            throw new UnauthorizedAccessException("You can only update your own comments.");
        
        updateComment.Update(request.Content);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}