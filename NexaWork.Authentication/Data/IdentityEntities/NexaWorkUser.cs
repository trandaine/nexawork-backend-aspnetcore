using Microsoft.AspNetCore.Identity;

namespace NexaWork.Authentication.Data.IdentityEntities;

public class NexaWorkUser : IdentityUser
{
    public string? Avatar { get; set; }
}
