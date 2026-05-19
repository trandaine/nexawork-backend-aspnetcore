using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Commands.UpdateName;

public record UpdateCustomerNameCommand(
    // string IdentityUserId,
    string FirstName,
    string LastName
) : IRequest;