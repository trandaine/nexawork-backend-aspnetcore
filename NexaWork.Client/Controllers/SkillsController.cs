using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.Features.Client.Skill.Commands.Create;
using NexaWork.Application.Features.Client.Skill.Commands.Delete;
using NexaWork.Application.Features.Client.Skill.Commands.Update;
using NexaWork.Application.Features.Client.Skill.Queries;
using NexaWork.Application.Features.Client.Skill.Queries.GetAll;
using NexaWork.Application.Features.Client.Skill.Queries.GetById;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillsController : ControllerBase
    {
        private readonly ISender _mediator;

        public SkillsController(ISender mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<SkillsController>
        [HttpGet]
        public async Task<ActionResult<List<SkillQueryDTO>>> Get()
        {
            var skills = await _mediator.Send(new GetAllSkillsQuery());
            return Ok(skills);
        }
        
        // GET api/<SkillsController>/5
        [HttpGet("id/{id:guid}")]
        public async Task<ActionResult<SkillQueryDTO>> Get(Guid id)
        {
            var skill = await _mediator.Send(new GetSkillByIdQuery(id));
            return Ok(skill);
        }

        // POST api/<SkillsController>
        [HttpPost("new")]
        public async Task<ActionResult<Guid>> Create(CreateSkillCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // PUT api/<SkillsController>/5
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSkillCommand command)
        {
            if (id != command.SkillId)
            {
                return BadRequest("ID in URL does not match ID in request body");
            }

            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE api/<SkillsController>/5
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteSkillCommand(id);
            await _mediator.Send(command);
            
            return NoContent();
        }
    }
}