using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Application.Common.Behaviors;

public class UserEnrichmentBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;

    public UserEnrichmentBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (request is IUserRequest userRequest)
        {
            // If the UserId is already populated (e.g. by a background worker), do not overwrite it.
            if (string.IsNullOrEmpty(userRequest.UserId))
            {
                var userId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                userRequest.UserId = userId;
            }
        }

        return await next();
    }
}
