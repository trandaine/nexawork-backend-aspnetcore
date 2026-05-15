using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.DTOs.Post;
using NexaWork.Application.Features.Client.Post.Commands.Create;
using NexaWork.Application.Features.Client.Post.Commands.Delete;
using NexaWork.Application.Features.Client.Post.Commands.Update;
using NexaWork.Application.Features.Client.Post.Queries;
using NexaWork.Application.Features.Client.Post.Queries.GetAll;
using NexaWork.Application.Features.Client.Post.Queries.GetById;
using NexaWork.Client.Models;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly ISender _mediator;

        public PostsController(
            ISender mediator
            )
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Create new post method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromForm] CreatePostRequest request)
        {
            // Extract the CustomerId from the JWT Token Claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the claim exists and is a valid GUID, then parsed into customerId
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var customerId))
            {
                return Unauthorized("User ID not found in token.");
            }

            FileDTO? fileDto = null;

            // Translate IFormFile to our pure FileDto
            if (request.MediaFile != null)
            {
                fileDto = new FileDTO(
                    request.MediaFile.OpenReadStream(),
                    request.MediaFile.FileName,
                    request.MediaFile.ContentType,
                    request.MediaFile.Length
                );
            }


            // Send the command to the application layer
            var command = new CreatePostCommand(
                userIdClaim, // Pass the IdentityUserId (string)
                customerId,
                request.Content,
                fileDto,
                request.Visibility
            );

            var postId = await _mediator.Send(command);
            return Ok(postId);
        }

        /// <summary>
        /// Get all posts method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PostQueryDTO>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllPostsQuery());
            return Ok(result);
        }



        /// <summary>
        /// Get post by ID method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PostQueryDTO?>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPostByIdQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }



        /// <summary>
        /// Method to update an existing post. Pass into the post ID. 
        /// The update is like the same of create, so can reuse the CreatePostRequest as input model.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreatePostRequest request)
        {
            // Extract Identity ID from JWT
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            // Map the incoming request file to FileDTO (like you did in Create)
            FileDTO? fileDto = null;
            if (request.MediaFile != null)
            {
                using var stream = request.MediaFile.OpenReadStream();
                fileDto = new FileDTO(
                    stream,
                    request.MediaFile.FileName,
                    request.MediaFile.ContentType,
                    request.MediaFile.Length
                );
            }

            // Assemble the command
            var command = new UpdatePostCommand(
                id, // The ID from the URL path!
                userIdClaim,
                request.Content,
                fileDto,
                request.Visibility
            );

            await _mediator.Send(command);

            return NoContent();
        }
        
        
        /// <summary>
        /// Delete post by ID method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var command = new DeletePostCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
