using MediatR;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Create;

public record CreateCustomerAddressCommand(string? City, string? PostalCode, string? Country, string? TaxId) : IRequest<Guid>;