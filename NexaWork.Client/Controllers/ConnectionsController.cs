using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.DTOs.Connections;
using NexaWork.Application.Features.Client.Connections.Commands.AcceptRequest;
using NexaWork.Application.Features.Client.Connections.Commands.BlockConnection;
using NexaWork.Application.Features.Client.Connections.Commands.RejectRequest;
using NexaWork.Application.Features.Client.Connections.Commands.RemoveConnection;
using NexaWork.Application.Features.Client.Connections.Commands.SendRequest;
using NexaWork.Application.Features.Client.Connections.Commands.UnblockConnection;
using NexaWork.Application.Features.Client.Connections.Commands.UndoRequest;
using NexaWork.Application.Features.Client.Connections.Queries.GetConnections;
using NexaWork.Application.Features.Client.Connections.Queries.GetPendingConnections;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConnectionsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ConnectionsController(ISender mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Send connection command to target customer
        /// </summary>
        /// <param name="targetCustomerId"></param>
        /// <returns></returns>
        [HttpPost("{targetCustomerId:guid}/send")]
        public async Task<ActionResult> SendRequest(Guid targetCustomerId)
        {
            await _mediator.Send(new SendConnectionRequestCommand(targetCustomerId));
            return NoContent();
        }
        
        
        /// <summary>
        /// Send accept connection command to customer's connection request
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        [HttpPut("{connectionId:guid}/accept")]
        public async Task<ActionResult> AcceptRequest(Guid connectionId)
        {
            await _mediator.Send(new AcceptConnectionRequestCommand(connectionId));
            return NoContent();
        }
        
        /// <summary>
        /// Send reject connection command to customer's connection request
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        [HttpPut("{connectionId:guid}/reject")]
        public async Task<ActionResult> RejectRequest(Guid connectionId)
        {
            await _mediator.Send(new RejectConnectionRequestCommand(connectionId));
            return NoContent();
        }
        
        /// <summary>
        /// Send block connection command to customer's connection request
        /// </summary>
        /// <param name="targetCustomerId"></param>
        /// <returns></returns>
        [HttpPut("{targetCustomerId:guid}/block")]
        public async Task<ActionResult> BlockConnection(Guid targetCustomerId)
        {
            await _mediator.Send(new BlockConnectionCommand(targetCustomerId));
            return NoContent();
        }
        
        /// <summary>
        /// Unblock a previously blocked connection with the target customer
        /// </summary>
        /// <param name="targetCustomerId"></param>
        /// <returns></returns>
        [HttpPut("{targetCustomerId:guid}/unblock")]
        public async Task<ActionResult> UnblockConnection(Guid targetCustomerId)
        {
            await _mediator.Send(new UnblockConnectionCommand(targetCustomerId));
            return NoContent();
        }
        
        /// <summary>
        /// Undo a pending connection request that was sent to the target customer
        /// </summary>
        /// <param name="targetCustomerId"></param>
        /// <returns></returns>
        [HttpDelete("{targetCustomerId:guid}/undo")]
        public async Task<ActionResult> UndoRequest(Guid targetCustomerId)
        {
            await _mediator.Send(new UndoConnectionRequestCommand(targetCustomerId));
            return NoContent();
        }
        
        /// <summary>
        /// Remove an accepted connection with the target customer
        /// </summary>
        /// <param name="targetCustomerId"></param>
        /// <returns></returns>
        [HttpDelete("{targetCustomerId:guid}/remove")]
        public async Task<ActionResult> RemoveConnection(Guid targetCustomerId)
        {
            await _mediator.Send(new RemoveConnectionCommand(targetCustomerId));
            return NoContent();
        }
        
        
        /// <summary>
        /// Get Request list that are pending for the current user. These are the requests that the current user has received but has not yet accepted or rejected.
        /// </summary>
        /// <returns></returns>
        [HttpGet("pending")]
        public async Task<ActionResult<List<ConnectionDto>>> GetPendingRequests()
        {
            var result = await _mediator.Send(new GetPendingConnectionsQuery());
            return Ok(result);
        }
        
        
        /// <summary>
        /// Get all connected customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ConnectionDto>>> GetConnections()
        {
            var result = await _mediator.Send(new GetConnectionsQuery());
            return Ok(result);
        }
    }
}
