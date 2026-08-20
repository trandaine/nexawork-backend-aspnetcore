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

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

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

    [HttpPut("{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead([FromRoute] Guid messageId)
    {
        await _mediator.Send(new MarkMessageAsReadCommand(messageId));
        return NoContent();
    }

    [HttpPut("{otherCustomerId}/read-all")]
    public async Task<IActionResult> MarkConversationAsRead([FromRoute] Guid otherCustomerId)
    {
        await _mediator.Send(new MarkConversationAsReadCommand(otherCustomerId));
        return NoContent();
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid messageId)
    {
        await _mediator.Send(new DeleteMessageCommand(messageId));
        return NoContent();
    }
}
