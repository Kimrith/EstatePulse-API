using EstatePulse.Common.Results;
using EstatePulse.Modules.Projects.Commands.Create;
using EstatePulse.Modules.Projects.Commands.Delete;
using EstatePulse.Modules.Projects.Commands.Update;
using EstatePulse.Modules.Projects.DTOs;
using EstatePulse.Modules.Projects.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Projects.Controllers;

[ApiController]
[Route("api/project-phases")]
public class ProjectPhasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectPhasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/project-phases?projectId=1&pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProjectPhaseDto>>> GetAll(
        [FromQuery] int? projectId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProjectPhasesQuery(projectId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    // GET: api/project-phases/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectPhaseDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var phase = await _mediator.Send(new GetProjectPhaseByIdQuery(id), cancellationToken);

        if (phase == null)
        {
            return NotFound(new { message = $"Project phase with ID {id} was not found." });
        }

        return Ok(phase);
    }

    // GET: api/project-phases/by-slug/1/phase-1
    [HttpGet("by-slug/{projectId:int}/{slug}")]
    public async Task<ActionResult<ProjectPhaseDto>> GetBySlug(int projectId, string slug, CancellationToken cancellationToken = default)
    {
        var phase = await _mediator.Send(new GetProjectPhaseBySlugQuery(projectId, slug), cancellationToken);

        if (phase == null)
        {
            return NotFound(new { message = $"Project phase '{slug}' in project {projectId} was not found." });
        }

        return Ok(phase);
    }

    // POST: api/project-phases (multipart/form-data)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProjectPhaseDto>> Create(
        [FromForm] CreateProjectPhaseDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateProjectPhaseCommand(
                dto.ProjectId,
                dto.PhaseName,
                dto.Slug,
                dto.MasterPlanSvgFile,
                dto.LaunchDate,
                dto.ExpectedCompletionDate,
                dto.Status
            );
            var createdPhase = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdPhase.Id }, createdPhase);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/project-phases/5 (multipart/form-data)
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProjectPhaseDto>> Update(
        int id,
        [FromForm] UpdateProjectPhaseDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateProjectPhaseCommand(
                id,
                dto.ProjectId,
                dto.PhaseName,
                dto.Slug,
                dto.MasterPlanSvgFile,
                dto.LaunchDate,
                dto.ExpectedCompletionDate,
                dto.Status
            );

            var updatedPhase = await _mediator.Send(command, cancellationToken);

            if (updatedPhase == null)
            {
                return NotFound(new { message = $"Project phase with ID {id} was not found." });
            }

            return Ok(updatedPhase);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/project-phases/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _mediator.Send(new DeleteProjectPhaseCommand(id), cancellationToken);

        if (!deleted)
        {
            return NotFound(new { message = $"Project phase with ID {id} was not found." });
        }

        return NoContent();
    }
}
