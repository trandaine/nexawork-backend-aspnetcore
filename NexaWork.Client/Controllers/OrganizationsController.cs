using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Features.Client.Organization.Commands.Create;
using NexaWork.Application.Features.Client.Organization.Commands.Update;
using NexaWork.Application.Features.Client.Organization.Queries;
using NexaWork.Application.Features.Client.Organization.Queries.GetAll;
using NexaWork.Application.Features.Client.Organization.Queries.GetById;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly INexaWorkDbContext _context;
        private readonly ISender _mediator;

        public OrganizationsController(
            ISender mediator,
            INexaWorkDbContext context
            )
        {
            _context = context;
            _mediator = mediator;
        }

        // GET: api/Organizations
        [HttpGet]
        public async Task<ActionResult<List<OrganizationQueryDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllOrganizationsQuery());
            return Ok(result);
        }


        // GET: api/Organizations/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrganizationQueryDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrganizationByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        // PUT: api/Organizations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateOrganizationCommand command)
        {
            if (id != command.OrganizationId)
            {
                return BadRequest("ID in URL does not match ID in request body");
            }

            await _mediator.Send(command);
            return NoContent(); // Return 204 No Content to indicate successful update without returning data

        }


        // POST: api/Organizations
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateOrganizationCommand command)
        {
            var organizationId = await _mediator.Send(command);
            return Ok(organizationId);
        }

        // DELETE: api/Organizations/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteOrganization(Guid id)
        // {
        //     var organization = await _context.Organizations.FindAsync(id);
        //     if (organization == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Organizations.Remove(organization);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool OrganizationExists(Guid id)
        {
            return _context.Organizations.Any(e => e.OrganizationId == id);
        }
    }
}
