using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Features.Client.CustomerAddress.Commands.Create;
using NexaWork.Application.Features.Client.CustomerAddress.Commands.Update;
using NexaWork.Application.Features.Client.CustomerAddress.Queries;
using NexaWork.Application.Features.Client.CustomerAddress.Queries.GetById;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomerAddressesController(ISender mediator)
        {
            _mediator = mediator;
        }

        // GET api/<CustomerAddress>/5
        [HttpGet("get-customer-address")]
        public async Task<ActionResult<CustomerAddressQueryDTO>> GetCustomerAddress()
        {
            var result = await _mediator.Send(new GetCustomerAddressByIdQuery());
            return Ok(result);
        }

        // POST api/<CustomerAddress>
        // [HttpPost("create-customer-address")]
        // public async Task<ActionResult<Guid>> CreateCustomerAddress([FromBody] CreateCustomerAddressCommand command)
        // {
        //     var newCustomerAddressId = await _mediator.Send(command);
        //     return Ok(newCustomerAddressId);
        // }

        // PUT api/<CustomerAddress>/5
        [HttpPut("update-customer-address")]
        public async Task<IActionResult> UpdateCustomerAddress([FromBody] UpdateCustomerAddressCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        // // DELETE api/<CustomerAddress>/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}