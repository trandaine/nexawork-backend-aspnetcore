using NexaWork.Application.Common.Interfaces;
﻿using MediatR;

namespace NexaWork.Application.Features.Client.Post.Queries.GetAllCurrentCustomer;

public record GetAllCurrentCustomerQuery() : IRequest<List<PostQueryDTO>>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}