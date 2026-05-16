using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Comment.Commands.Delete;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public DeleteCommentHandler(
        ICommentRepository commentRepository,
        INexaWorkDbContext unitOfWork
    )
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var deleteComment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (deleteComment == null)
        {
            throw new Exception($"Comment with ID {request.CommentId} not found");
        }

        _commentRepository.Remove(deleteComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}