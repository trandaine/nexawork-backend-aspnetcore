using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Features.Client.Customers.Queries;
using NexaWork.Application.Features.Client.Customers.Queries.GetAll;
using NexaWork.Application.Features.Client.Customers.Queries.GetById;
using NexaWork.Application.Features.Client.Customers.Queries.GetByIdentityId;

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

        // GET: api/Customers/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerQueryDTO>> GetCustomerById(Guid id)
        {
            var customer = await _mediator.Send(new GetCustomerByIdQuery(id));
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }


        // GET: api/Customers/5
        [HttpGet("me/{identityId}")]
        public async Task<ActionResult<CustomerWithIdentityIdDTO>> GetCustomerByIdentityId(string identityId)
        {
            var customer = await _mediator.Send(new GetCustomerByIdentityIdQuery(identityId));
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }



    }
}
