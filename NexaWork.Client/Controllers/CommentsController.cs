using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Features.Client.Comment.Commands.Create;
using NexaWork.Application.Features.Client.Comment.Commands.Delete;
using NexaWork.Application.Features.Client.Comment.Commands.Update;
using NexaWork.Application.Features.Client.Comment.Queries;
using NexaWork.Application.Features.Client.Comment.Queries.GetAllCommentsByPostId;

namespace NexaWork.Client.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CommentsController : Controller
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost("new-comment")]
    public async Task<IActionResult> Create(Guid postId, string content)
    {
        var commentId = await _mediator.Send(new CreateCommentCommand(postId, content));
        return Ok(commentId);
    }
    
    [HttpPut("update/{postId}")]
    public async Task<IActionResult> Update(Guid commentId, [FromBody] UpdateCommentCommand command)
    {
        if (commentId != command.CommentId)
            return BadRequest("Id in URL does not match Id in request body");
        await _mediator.Send(command);        
        return NoContent();
    }
    
    [HttpDelete("delete/{postId}")]
    public async Task<IActionResult> Delete(Guid commentId)
    {
        await _mediator.Send(new DeleteCommentCommand(commentId));
        return NoContent(); 
    }
    
    [HttpGet("{postId}")]
    public async Task<ActionResult<List<CommentQueryDTO>>> GetAllCommentsByPostId(Guid postId)
    {
        // This method would call a query to get comments by post ID
        var comments = await _mediator.Send(new GetAllCommentsByPostIdQuery(postId));
        // return Ok(comments);
        return Ok(comments); // Placeholder
    }
}