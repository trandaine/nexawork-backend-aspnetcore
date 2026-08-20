using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Education.Commands.Update;

public record UpdateEducationCommand(
    string SchoolName,
    string? Degree,
    string? FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description) : IRequest, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}