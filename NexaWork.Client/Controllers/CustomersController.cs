using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.DTOs;
// using NexaWork.Application.DTOs.Customer;
using NexaWork.Application.Features.Client.Customers.Commands.Update;
using NexaWork.Application.Features.Client.Customers.Commands.UpdateName;
using NexaWork.Application.Features.Client.Customers.Queries;
using NexaWork.Application.Features.Client.Customers.Queries.GetAll;
using NexaWork.Application.Features.Client.Customers.Queries.GetById;
using NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;
using NexaWork.Client.Models;
using NexaWork.Client.Models.CustomerDTOs;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomersController(ISender mediator)
        {
            _mediator = mediator;
        }


        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<List<CustomerQueryDTO>>> GetAll()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(customers);
        }

        /// <summary>
        /// Get current user complete profile information
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile-me")]
        public async Task<ActionResult<CustomerQueryDTO>> GetCurrentCustomer()
        {
            // var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            //                      ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            //
            // if (string.IsNullOrEmpty(identityUserId))
            // {
            //     return Unauthorized("User ID not found in token.");
            // }

            var customer = await _mediator.Send(new GetCustomerByIdQuery());
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }


        // GET: api/Customers/5
        // [HttpGet("me/{identityId}")]
        // public async Task<ActionResult<CustomerWithIdentityIdDTO>> GetCustomerByIdentityId(string identityId)
        // {
        //     var customer = await _mediator.Send(new GetCustomerByIdentityIdQuery(identityId));
        //     if (customer == null)
        //         return NotFound();
        //     return Ok(customer);
        // }

        /// <summary>
        /// GET CURRENT CUSTOMER INFORMATION WITH TOKEN.
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        public async Task<ActionResult<CustomerWithIdentityIdDTO>> GetCurrentCustomerName()
        {
            var customer = await _mediator.Send(new GetCustomerByIdentityIdQuery());

            if (customer == null)
            {
                return NotFound("Customer profile not found for this account.");
            }

            return Ok(customer);
        }

        /// <summary>
        /// Update customer information
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update-customer-name")]
        public async Task<ActionResult> UpdateCustomer([FromBody] UpdateCustomerNameRequestDTO request)
        {
            var command = new UpdateCustomerNameCommand(
                request.FirstName,
                request.LastName
            );

            // 4. Execute the command
            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Update entire customer profile, including profile picture and background picture.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update-customer-profile")]
        public async Task<IActionResult> UpdateCustomerProfile([FromForm] CustomerRequestDTO request)
        {
            FileDTO? profilePictureFileDto = null;
            FileDTO? backgroundPictureFileDto = null;

            if (request.ProfilePictureFile != null)
            {
                profilePictureFileDto = new FileDTO(
                    request.ProfilePictureFile.OpenReadStream(),
                    request.ProfilePictureFile.FileName,
                    request.ProfilePictureFile.ContentType,
                    request.ProfilePictureFile.Length
                );
            }

            if (request.BackgroundPictureFile != null)
            {
                backgroundPictureFileDto = new FileDTO(
                    request.BackgroundPictureFile.OpenReadStream(),
                    request.BackgroundPictureFile.FileName,
                    request.BackgroundPictureFile.ContentType,
                    request.BackgroundPictureFile.Length
                );
            }

            var command = new UpdateCustomerCommand(
                request.FirstName,
                request.LastName,
                request.Headline,
                request.Summary,
                request.Location,
                backgroundPictureFileDto,
                profilePictureFileDto
            );

            await _mediator.Send(command);

            return NoContent();
        }
    }
}