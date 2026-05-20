namespace NexaWork.Application.Features.Client.CustomerAddress.Queries;

public record CustomerAddressQueryDTO(
    string? CustomerId,
    string? AddressId,
    string? City,
    string? State);
