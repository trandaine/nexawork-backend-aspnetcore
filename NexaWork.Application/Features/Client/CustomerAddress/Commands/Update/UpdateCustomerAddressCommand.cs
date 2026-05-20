using MediatR;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Update;

public record UpdateCustomerAddressCommand(string? City, string? PostalCode, string? Country, string? TaxId) : IRequest;