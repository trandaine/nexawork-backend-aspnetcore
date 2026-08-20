using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.CustomerAddress.Commands.Create;

public record CreateCustomerAddressCommand() : IRequest<Guid>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}