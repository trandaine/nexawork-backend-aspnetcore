using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NexaWork.Domain.Entities;

public class Customer
{
    public Guid CustomerId { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Description("Phần bio của Profile Người dùng")]
    public string? Headline { get; set; }
    public string? Summary { get; set; }
    public string? Location { get; set; }

    [Description("Ảnh đại diện của Profile Người dùng")]
    public string? ProfilePictureUrl { get; set; }

    [Description("Ảnh bìa của Profile Người dùng")]
    public string? BackgroundPictureUrl { get; set; }


    #region Navigation Properties

    public virtual ICollection<Connection> SentConnections { get; set; } = new Collection<Connection>();
    public virtual ICollection<Connection> ReceivedConnections { get; set; } = new Collection<Connection>();
    public virtual ICollection<Education> Educations { get; set; } = new Collection<Education>();
    public virtual ICollection<Experience> Experiences { get; set; } = new Collection<Experience>();
    public virtual ICollection<CustomerSkill> CustomerSkills { get; set; } = new Collection<CustomerSkill>();
    public virtual ICollection<Post> Posts { get; set; } = new Collection<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new Collection<Comment>();
    public virtual ICollection<Reaction> Reactions { get; set; } = new Collection<Reaction>();
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new Collection<JobApplication>();

    #endregion
}
