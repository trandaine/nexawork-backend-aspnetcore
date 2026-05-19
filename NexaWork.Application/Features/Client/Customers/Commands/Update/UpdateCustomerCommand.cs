using MediatR;
using NexaWork.Application.DTOs;
// using NexaWork.Application.DTOs.Customer;

namespace NexaWork.Application.Features.Client.Customers.Commands.Update;

public record UpdateCustomerCommand(
    string IdentityUserId,
    // Guid CustomerId,
    string FirstName,
    string LastName,
    string? Headline,
    string? Summary,
    string? Location,
    FileDTO? BackgroundPictureFile,
    FileDTO? ProfilePictureFile) : IRequest;