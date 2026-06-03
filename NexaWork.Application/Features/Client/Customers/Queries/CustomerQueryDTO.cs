using System;

namespace NexaWork.Application.Features.Client.Customers.Queries;

public record CustomerQueryDTO
(
    Guid CustomerId,
    // string IdentityUserId,
    string? FirstName,
    string? LastName,
    string? Headline,
    string? Summary,
    string? Location,
    string? ProfilePictureUrl,
    string? BackgroundPictureUrl,
    string? PhoneNumber
);
