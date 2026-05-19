namespace NexaWork.Domain.Entities;

public class CustomerAddress
{
    public Guid CustomerAddressId { get; private set; }
    public string? City { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? TaxId { get; private set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public Guid CustomerId { get; private set; }

    // Navigation property
    public Customer Customer { get; private set; } = null!;
}