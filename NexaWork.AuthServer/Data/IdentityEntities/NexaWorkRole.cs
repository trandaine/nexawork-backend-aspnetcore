using Microsoft.AspNetCore.Identity;

namespace NexaWork.AuthServer.Data.IdentityEntities;

public class NexaWorkRole : IdentityRole
{
    public NexaWorkRole() : base()
    { }

    public NexaWorkRole(string roleName) : base(roleName)
    { }
    public string? Description { get; set; }
}
