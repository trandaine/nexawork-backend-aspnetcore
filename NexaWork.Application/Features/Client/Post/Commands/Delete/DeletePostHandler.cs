using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;

namespace NexaWork.Application.Features.Client.Post.Commands.Delete;

public class DeletePostHandler : IRequestHandler<DeletePostCommand> 
{
    private readonly IPostRepository _postRepository;
    private readonly INexaWorkDbContext _unitOfWork;

    public DeletePostHandler(
        IPostRepository postRepository,
        INexaWorkDbContext unitOfWork
        )
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            throw new Exception($"Post with ID {request.PostId} not found");
        }
        _postRepository.Remove(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
