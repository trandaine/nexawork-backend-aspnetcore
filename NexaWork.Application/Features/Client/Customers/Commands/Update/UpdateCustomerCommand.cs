using MediatR;

namespace NexaWork.Application.Features.Client.Customers.Commands.Update;

public record UpdateCustomerCommand(
    string IdentityUserId,
    Guid CustomerId,
    string FirstName,
    string LastName,
    string? Headline,
    string? Summary,
    string? Location) : IRequest;