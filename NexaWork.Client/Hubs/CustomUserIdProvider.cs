using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Abstractions;

namespace NexaWork.Client.Hubs;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Check OpenIddict subject claim first, then standard NameIdentifier
        return connection.User?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value
            ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
