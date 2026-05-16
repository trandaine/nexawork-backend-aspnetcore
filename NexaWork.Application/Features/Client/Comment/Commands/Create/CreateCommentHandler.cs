using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Comment.Commands.Create;

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICommentRepository _commentRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public CreateCommentHandler(
        ICommentRepository commentRepository,
        INexaWorkDbContext unitOfWork
        )
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var newComment = NexaWork.Domain.Entities.Comment.Create(
            request.PostId,
            request.CustomerId,
            request.Content
        );
        
        _commentRepository.Add(newComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newComment.CommentId;
    }
}