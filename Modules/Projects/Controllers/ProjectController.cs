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
        var result = await _mediator.Send(new GetAllProjectsQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    // GET: api/project/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(id), cancellationToken);

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
        var project = await _mediator.Send(new GetProjectBySlugQuery(slug), cancellationToken);

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
            var command = new CreateProjectCommand(
                dto.Name,
                dto.Slug,
                dto.LocationAddress,
                dto.City,
                dto.Latitude,
                dto.Longitude,
                dto.ThumbnailFile,
                dto.Description,
                dto.IsPublished
            );
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
            var command = new UpdateProjectCommand(
                id,
                dto.Name,
                dto.Slug,
                dto.LocationAddress,
                dto.City,
                dto.Latitude,
                dto.Longitude,
                dto.ThumbnailFile,
                dto.Description,
                dto.IsPublished
            );

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
        var deleted = await _mediator.Send(new DeleteProjectCommand(id), cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
