using System.ComponentModel.DataAnnotations;

namespace NexaWork.Authentication.Data.IdentityEntities;

public class FidoStoredCredential
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;

    public byte[]? PublicKey { get; set; }
    public byte[]? UserHandle { get; set; }
    public uint SignatureCounter { get; set; }
    
    [Required]
    public string CredType { get; set; } = string.Empty;
    public DateTime RegDate { get; set; }
    public Guid AaGuid { get; set; }
    
    // The FIDO2 CredentialId (a byte array) encoded or stored natively. We store as byte[]
    public byte[]? DescriptorId { get; set; }

    // Optional user-friendly name for this passkey
    public string? DisplayName { get; set; }

    public NexaWorkUser? User { get; set; }
}
