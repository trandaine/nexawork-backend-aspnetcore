using MediatR;

namespace NexaWork.Application.Features.Client.Education.Queries.GetById;

public record GetEducationByIdQuery() : IRequest<EducationQueryDTO?>;