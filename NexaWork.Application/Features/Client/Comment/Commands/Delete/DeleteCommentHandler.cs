using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Features.Client.Comment.Commands.Delete;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly INexaWorkDbContext _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerRepository _customerRepository;

    public DeleteCommentHandler(
        ICurrentUserService currentUserService,
        ICustomerRepository customerRepository,
        ICommentRepository commentRepository,
        INexaWorkDbContext unitOfWork
    )
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _customerRepository = customerRepository;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        // Guard 1: Does the acting user exist?
        var customer = await _customerRepository.GetByIdentityIdAsync(userId, cancellationToken);
        if (customer == null)
            throw new UnauthorizedAccessException("Customer profile not found for this user.");

        // Guard 2: Does the target comment exist?
        var deleteComment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (deleteComment == null)
            throw new KeyNotFoundException($"Comment with ID {request.CommentId} not found");

        // Guard 3: Does the user own the comment?
        if (deleteComment.CustomerId != customer.CustomerId)
            throw new UnauthorizedAccessException("You can only delete your own comments.");

        _commentRepository.Remove(deleteComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}