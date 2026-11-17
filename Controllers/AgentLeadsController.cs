using EstatePulse.Commands.Create;
using EstatePulse.Commands.Delete;
using EstatePulse.Commands.Update;
using EstatePulse.DTOs;
using EstatePulse.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Controllers
{
    [Route("api/agent-leads")]
    [ApiController]
    public class AgentLeadsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentLeadsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/agent-leads?agentId=1&status=New&searchTerm=John&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PaginatedList<AgentLeadDto>>> GetLeads(
            [FromQuery] int? agentId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new AgentLeadQueries.GetPaginatedAgentLeadsQuery
            {
                AgentId = agentId,
                Status = status,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/agent-leads/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AgentLeadDto>> GetLeadById(int id)
        {
            var query = new AgentLeadQueries.GetAgentLeadByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Lead with ID {id} not found." });
            }

            return Ok(result);
        }

        // POST: api/agent-leads
        [HttpPost]
        public async Task<ActionResult<AgentLeadDto>> CreateLead([FromBody] AgentLeadCreateDto dto)
        {
            try
            {
                var command = new CreateAgentLeadCommand { Lead = dto };
                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetLeadById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/agent-leads/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<AgentLeadDto>> UpdateLead(int id, [FromBody] AgentLeadUpdateDto dto)
        {
            try
            {
                var command = new UpdateAgentLeadCommand { Id = id, Lead = dto };
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    return NotFound(new { message = $"Lead with ID {id} not found." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: api/agent-leads/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<AgentLeadDto>> UpdateLeadStatus(int id, [FromBody] AgentLeadStatusUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new { message = "Status cannot be empty." });
            }

            var updateDto = new AgentLeadUpdateDto { Status = dto.Status.Trim() };
            var command = new UpdateAgentLeadCommand { Id = id, Lead = updateDto };
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound(new { message = $"Lead with ID {id} not found." });
            }

            return Ok(result);
        }

        // DELETE: api/agent-leads/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead(int id)
        {
            var command = new DeleteAgentLeadCommand(id);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new { message = $"Lead with ID {id} not found." });
            }

            return NoContent();
        }
    }
}
