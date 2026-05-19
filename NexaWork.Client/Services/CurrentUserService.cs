using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NexaWork.Application.Common.Interfaces.Services;

namespace NexaWork.Client.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId 
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var id = user?.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(id))
            {
                // This exception will be caught globally by your API and turned into a 401 response!
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return id;
        }
    }
}