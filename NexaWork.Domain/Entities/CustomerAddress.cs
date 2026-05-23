namespace NexaWork.Domain.Entities;

public class CustomerAddress
{
    public Guid CustomerAddressId { get; private set; }
    public string? City { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? TaxId { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime? DateUpdated { get; private set; }
    public Guid CustomerId { get; private set; }

    // Navigation property
    public Customer Customer { get; private set; } = null!;

    private CustomerAddress()
    {
    }

    public static CustomerAddress Create(Guid customerId)
    {
        return new CustomerAddress()
        {
            CustomerAddressId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            CustomerId = customerId
        };
    }
    
    public void Update(string? city, string? postalCode, string? country, string? taxId)
    {
        City = city;
        PostalCode = postalCode;
        Country = country;
        TaxId = taxId;
        DateUpdated = DateTime.UtcNow;
    }
}