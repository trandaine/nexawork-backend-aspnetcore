using System;
using Microsoft.AspNetCore.Identity;

namespace NexaWork.Domain.IdentityEntites;

public class NexaWorkUser : IdentityUser
{
    public string? Avatar { get; set; }
}
