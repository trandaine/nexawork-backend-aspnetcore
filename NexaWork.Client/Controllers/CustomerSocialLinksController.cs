using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;
using NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Update;
using NexaWork.Application.Features.Client.CustomerSocialLink.Queries;
using NexaWork.Application.Features.Client.CustomerSocialLink.Queries.GetById;
using NexaWork.Domain.Entities;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerSocialLinksController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomerSocialLinksController(ISender mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("get-customer-social-links")]
        public async Task<ActionResult<CustomerSocialLinkQueryDTO>> Get()
        {
            var result = await _mediator.Send(new GetCustomerSocialLinkByIdQuery());
            return Ok(result);
        }

        // POST api/<CustomerSocialLinksController>
        // [HttpPost("create-customer-social-link")]
        // public async Task<IActionResult> Post([FromBody] CreateCustomerSocialLinkCommand command)
        // {
        //     var newCustomerSocialLinkId = await _mediator.Send(command);
        //     return Ok(newCustomerSocialLinkId);
        // }


        // PUT api/<CustomerSocialLinksController>/5
        [HttpPut("update-customer-social-link")]
        public async Task<IActionResult> Put([FromBody] UpdateCustomerSocialLinkCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}