using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Education.Commands.Create;

public record CreateEducationCommand( 
    string SchoolName,
    string? Degree,
    string? FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description) : IRequest<Guid>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}