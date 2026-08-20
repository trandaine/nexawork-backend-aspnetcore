using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Commands.UpdateName;

public record UpdateCustomerNameCommand(
    // 
    string FirstName,
    string LastName
) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}