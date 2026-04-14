using System;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Domain.Enums;

namespace NexaWork.Admin.Models;

[Bind("JobListingId,OrganizationId,Title,Description,Requirements,Location,EmploymentType,SalaryRange,ContactEmail,CreatedAt,UpdatedAt,IsActive")]
public class JobListingViewModel
{
    public Guid JobListingId { get; set; }
    public Guid OrganizationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string? Location { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public string? SalaryRange { get; set; }
    public string? ContactEmail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
