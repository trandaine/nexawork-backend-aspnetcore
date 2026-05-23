namespace NexaWork.Application.Features.Client.CustomerAddress.Queries;

public record CustomerAddressQueryDTO(
    string? City,
    string? PostalCode,
    string? Country,
    string? TaxId);