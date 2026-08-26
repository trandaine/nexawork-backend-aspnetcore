using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.DTOs.Messages;
using NexaWork.Application.Features.Client.Messages.Commands.DeleteMessage;
using NexaWork.Application.Features.Client.Messages.Commands.MarkConversationAsRead;
using NexaWork.Application.Features.Client.Messages.Commands.MarkMessageAsRead;
using NexaWork.Application.Features.Client.Messages.Commands.SendMessage;
using NexaWork.Application.Features.Client.Messages.Queries.GetConversation;

namespace NexaWork.Client.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly ISender _mediator;

    public MessagesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sends a message to a customer.
    /// </summary>
    /// <param name="command">The command containing message details.</param>
    /// <returns>The created message details.</returns>
    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the conversation messages with a specific customer.
    /// </summary>
    /// <param name="otherCustomerId">The identifier of the other customer in the conversation.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of messages per page.</param>
    /// <returns>A list of messages in the conversation.</returns>
    [HttpGet("{otherCustomerId}")]
    public async Task<ActionResult<List<MessageDto>>> GetConversation(
        [FromRoute] Guid otherCustomerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetConversationQuery(otherCustomerId, page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Marks a specific message as read.
    /// </summary>
    /// <param name="messageId">The unique identifier of the message to mark as read.</param>
    /// <returns>An action result indicating the completion of the operation.</returns>
    [HttpPut("{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead([FromRoute] Guid messageId)
    {
        await _mediator.Send(new MarkMessageAsReadCommand(messageId));
        return NoContent();
    }

    /// <summary>
    /// Marks all messages in a conversation with a specific customer as read.
    /// </summary>
    /// <param name="otherCustomerId">The identifier of the other customer in the conversation.</param>
    /// <returns>An action result indicating the completion of the operation.</returns>
    [HttpPut("{otherCustomerId}/read-all")]
    public async Task<IActionResult> MarkConversationAsRead([FromRoute] Guid otherCustomerId)
    {
        await _mediator.Send(new MarkConversationAsReadCommand(otherCustomerId));
        return NoContent();
    }


    /// <summary>
    /// Deletes a specific message.
    /// </summary>
    /// <param name="messageId">The identifier of the message to delete.</param>
    /// <returns>An action result indicating the outcome of the operation.</returns>
    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid messageId)
    {
        await _mediator.Send(new DeleteMessageCommand(messageId));
        return NoContent();
    }
}
