using System;
using System.ComponentModel;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Connection
{
    public Guid ConnectionId { get; set; }
    [Description("ID của Customer chủ sở hữu kết nối")]
    public Guid CustomerId { get; set; }
    [Description("ID của Customer được kết nối")]
    public Guid ConnectedCustomerId { get; set; }
    public ConnectionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    #region Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Customer ConnectedCustomer { get; set; } = null!; 
    #endregion
}
