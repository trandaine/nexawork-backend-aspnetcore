using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.Education.Queries.GetById;

public record GetEducationByIdQuery() : IRequest<EducationQueryDTO?>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}