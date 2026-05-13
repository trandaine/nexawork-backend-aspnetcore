using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.DTOs.Post;
using NexaWork.Application.Features.Client.Post.Commands.Create;
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
    }
}
