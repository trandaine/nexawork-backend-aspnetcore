using MediatR;

namespace NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;

public record CreateCustomerSocialLinkCommand(
    string IdentityUserId ):IRequest<Guid>;