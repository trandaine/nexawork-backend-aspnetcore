using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NexaWork.Domain.Entities;

public class Customer
{
    public Guid CustomerId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    [Description("Phần bio của Profile Người dùng")]
    public string? Headline { get; private set; }
    public string? Summary { get; private set; }
    public string? Location { get; private set; }

    [Description("Ảnh đại diện của Profile Người dùng")]
    public string? ProfilePictureUrl { get; private set; }

    [Description("Ảnh bìa của Profile Người dùng")]
    public string? BackgroundPictureUrl { get; private set; }


    #region Navigation Properties

    public virtual ICollection<Connection> SentConnections { get; private set; } = new Collection<Connection>();
    public virtual ICollection<Connection> ReceivedConnections { get; private set; } = new Collection<Connection>();
    public virtual ICollection<Education> Educations { get; private set; } = new Collection<Education>();
    public virtual ICollection<Experience> Experiences { get; private set; } = new Collection<Experience>();
    public virtual ICollection<CustomerSkill> CustomerSkills { get; private set; } = new Collection<CustomerSkill>();
    public virtual ICollection<Post> Posts { get; private set; } = new Collection<Post>();
    public virtual ICollection<Comment> Comments { get; private set; } = new Collection<Comment>();
    public virtual ICollection<Reaction> Reactions { get; private set; } = new Collection<Reaction>();
    public virtual ICollection<JobApplication> JobApplications { get; private set; } = new Collection<JobApplication>();

    #endregion

    private Customer() { }

    public static Customer Create(
        string identityUserId
        )
    {
        return new Customer
        {
            CustomerId = Guid.NewGuid(),
            IdentityUserId = identityUserId,

        };
    }











}
