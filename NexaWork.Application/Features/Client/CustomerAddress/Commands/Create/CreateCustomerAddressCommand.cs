using MediatR;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Create;

public record CreateCustomerAddressCommand(string IdentityUserId) : IRequest<Guid>;