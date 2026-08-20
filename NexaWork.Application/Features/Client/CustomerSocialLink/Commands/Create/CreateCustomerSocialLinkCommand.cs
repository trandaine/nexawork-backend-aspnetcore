using NexaWork.Application.Common.Interfaces;
using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;

public record CreateCustomerSocialLinkCommand(
     ):IRequest<Guid>, IUserRequest
{
    public string UserId { get; set; } = string.Empty;
}