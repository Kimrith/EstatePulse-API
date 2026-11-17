using EstatePulse.Commands.Create;
using EstatePulse.Commands.Delete;
using EstatePulse.Commands.Update;
using EstatePulse.DTOs;
using EstatePulse.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/project?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProjectDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProjectsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // GET: api/project/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery { Id = id }, cancellationToken);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    // GET: api/project/slug/sunset-residences
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProjectDto>> GetBySlug(string slug, CancellationToken cancellationToken = default)
    {
        var project = await _mediator.Send(new GetProjectBySlugQuery { Slug = slug }, cancellationToken);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    // POST: api/project (multipart/form-data)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromForm] CreateProjectDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateProjectCommand { Project = dto };
            var createdProject = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, createdProject);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/project/5 (multipart/form-data)
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProjectDto>> Update(
        int id,
        [FromForm] UpdateProjectDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateProjectCommand
            {
                Id = id,
                Project = dto
            };

            var updatedProject = await _mediator.Send(command, cancellationToken);

            if (updatedProject == null)
            {
                return NotFound();
            }

            return Ok(updatedProject);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/project/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _mediator.Send(new DeleteProjectCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}