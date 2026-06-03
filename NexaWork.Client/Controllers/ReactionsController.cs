using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Features.Client.Reaction.Commands.Create;
using NexaWork.Application.Features.Client.Reaction.Commands.Delete;
using NexaWork.Application.Features.Client.Reaction.Queries.GetPostComment;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReactionsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ReactionsController(ISender mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("new-reaction{postId:guid}")]
        public async Task<IActionResult> CreateReaction(Guid postId)
        {
            await _mediator.Send( new CreateReactionCommand(postId));
            return NoContent();
        }

        [HttpDelete("delete-reaction{postId:guid}")]
        public async Task<IActionResult> DeleteReaction(Guid postId)
        {
            await _mediator.Send(new DeleteReactionCommand(postId));
            return NoContent();
        }
        
        [HttpGet("get-post-reaction{postId:guid}")]
        public async Task<bool> GetPostReaction(Guid postId)
        {
            var reaction = await _mediator.Send(new GetCustomerReactionByPostIdQuery(postId));
            return reaction;
        }
    }
}
