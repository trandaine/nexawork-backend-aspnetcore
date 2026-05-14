using Microsoft.AspNetCore.Identity;

namespace NexaWork.AuthServer.Data.IdentityEntities;

public class NexaWorkUser : IdentityUser
{
    public string? Avatar { get; set; }
}
