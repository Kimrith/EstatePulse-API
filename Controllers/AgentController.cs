using EstatePulse.Commands.Create;
using EstatePulse.Commands.Delete;
using EstatePulse.Commands.Update;
using EstatePulse.DTOs;
using EstatePulse.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/agents?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PaginatedList<AgentDto>>> GetAgents(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new AgentQueries.GetPaginatedAgentsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/agents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AgentDto>> GetAgentById(int id)
        {
            var query = new AgentQueries.GetAgentByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Agent with ID {id} not found." });
            }

            return Ok(result);
        }

        // POST: api/agents
        [HttpPost]
        public async Task<ActionResult<AgentDto>> CreateAgent([FromBody] AgentCreateDto dto)
        {
            try
            {
                var command = new CreateAgentCommand { Agent = dto };
                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetAgentById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/agents/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<AgentDto>> UpdateAgent(int id, [FromBody] AgentUpdateDto dto)
        {
            var command = new UpdateAgentCommand { Id = id, Agent = dto };
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound(new { message = $"Agent with ID {id} not found." });
            }

            return Ok(result);
        }

        // DELETE: api/agents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgent(int id)
        {
            var command = new DeleteAgentCommand(id);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new { message = $"Agent with ID {id} not found." });
            }

            return NoContent();
        }

        // GET: api/agents/{id}/leads
        [HttpGet("{id}/leads")]
        public async Task<ActionResult<PaginatedList<AgentLeadDto>>> GetAgentLeads(
            int id,
            [FromQuery] string? status = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new AgentLeadQueries.GetPaginatedAgentLeadsQuery
            {
                AgentId = id,
                Status = status,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}