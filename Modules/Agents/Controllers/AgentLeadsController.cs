using EstatePulse.Common.Results;
using EstatePulse.Modules.Agents.Commands.Create;
using EstatePulse.Modules.Agents.Commands.Delete;
using EstatePulse.Modules.Agents.Commands.Update;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using EstatePulse.Modules.Agents.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Agents.Controllers;

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
        var query = new GetPaginatedAgentLeadsQuery(
            agentId,
            status,
            searchTerm,
            pageNumber,
            pageSize
        );

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/agent-leads/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AgentLeadDto>> GetLeadById(int id)
    {
        var result = await _mediator.Send(new GetAgentLeadByIdQuery(id));

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
            var command = new CreateAgentLeadCommand(
                dto.AgentId,
                dto.CustomerId,
                dto.LeadName,
                dto.LeadPhone,
                dto.LeadEmail,
                dto.BudgetMin,
                dto.BudgetMax,
                dto.Status,
                dto.Notes
            );
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
            var command = new UpdateAgentLeadCommand(
                id,
                dto.CustomerId,
                dto.LeadName,
                dto.LeadPhone,
                dto.LeadEmail,
                dto.BudgetMin,
                dto.BudgetMax,
                dto.Status,
                dto.Notes
            );
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

        var command = new UpdateAgentLeadCommand(
            id,
            Status: dto.Status.Trim()
        );
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
        var success = await _mediator.Send(new DeleteAgentLeadCommand(id));

        if (!success)
        {
            return NotFound(new { message = $"Lead with ID {id} not found." });
        }

        return NoContent();
    }
}
