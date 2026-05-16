using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Comment.Commands.Update;

public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public UpdateCommentHandler(ICommentRepository commentRepository, INexaWorkDbContext unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var updateComment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (updateComment == null)
        {
            throw new InvalidOperationException("Comment not found.");
        }

        updateComment.Update(request.Content);
        
        _commentRepository.Update(updateComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}