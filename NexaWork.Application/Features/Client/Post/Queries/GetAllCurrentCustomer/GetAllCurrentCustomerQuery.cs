using MediatR;

namespace NexaWork.Application.Features.Client.Post.Queries.GetAllCurrentCustomer;

public record GetAllCurrentCustomerQuery() : IRequest<List<PostQueryDTO>>;