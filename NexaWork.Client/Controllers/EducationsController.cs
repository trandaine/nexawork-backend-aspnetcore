using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Features.Client.Education.Commands.Create;
using NexaWork.Application.Features.Client.Education.Commands.Update;
using NexaWork.Application.Features.Client.Education.Queries;
using NexaWork.Application.Features.Client.Education.Queries.GetById;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EducationsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EducationsController(ISender mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("get-customer-education")]
        public async Task<ActionResult<EducationQueryDTO>> GetById()
        {
            var education = await _mediator.Send(new GetEducationByIdQuery());
            return Ok(education);
        }

        [HttpPost("create-customer-education")]
        public async Task<ActionResult<Guid>> Create(CreateEducationCommand command)
        {
            var education = await _mediator.Send(command);
            return Ok(education);
        }

        [HttpPut("update-customer-education")]
        public async Task<IActionResult> Update(UpdateEducationCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
        
        
        
        
    }
}
