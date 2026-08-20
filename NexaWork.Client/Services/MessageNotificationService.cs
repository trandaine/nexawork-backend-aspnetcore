using Microsoft.AspNetCore.SignalR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Application.DTOs.Messages;
using NexaWork.Client.Hubs;

namespace NexaWork.Client.Services;

public class MessageNotificationService : IMessageNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ICustomerRepository _customerRepository;

    public MessageNotificationService(
        IHubContext<ChatHub> hubContext,
        ICustomerRepository customerRepository)
    {
        _hubContext = hubContext;
        _customerRepository = customerRepository;
    }

    public async Task NotifyNewMessage(Guid receiverCustomerId, MessageDto message, CancellationToken ct = default)
    {
        var receiver = await _customerRepository.GetCustomerByIdAsync(receiverCustomerId, ct);
        if (receiver == null) return;

        // OpenIddict subject claim corresponds to IdentityUserId
        await _hubContext.Clients.User(receiver.IdentityUserId)
            .SendAsync("ReceiveMessage", message, cancellationToken: ct);
    }

    public async Task NotifyMessageRead(Guid senderCustomerId, Guid messageId, DateTime readAt, CancellationToken ct = default)
    {
        var sender = await _customerRepository.GetCustomerByIdAsync(senderCustomerId, ct);
        if (sender == null) return;

        await _hubContext.Clients.User(sender.IdentityUserId)
            .SendAsync("MessageRead", new { messageId, readAt }, cancellationToken: ct);
    }

    public async Task NotifyMessageDeleted(Guid receiverCustomerId, Guid messageId, CancellationToken ct = default)
    {
        var receiver = await _customerRepository.GetCustomerByIdAsync(receiverCustomerId, ct);
        if (receiver == null) return;

        await _hubContext.Clients.User(receiver.IdentityUserId)
            .SendAsync("MessageDeleted", new { messageId }, cancellationToken: ct);
    }
}
