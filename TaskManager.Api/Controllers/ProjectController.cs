using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models.Domains.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(
        IProjectService projectService,
        ILogger<ProjectController> logger)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("create")]
    public async Task<ActionResult<Project>> CreateProject([FromBody] ProjectCreateDTO projectDto)
    {
        try
        {
            var project = await _projectService.CreateProjectAsync(projectDto);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        try
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            return Ok(project);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project not found with ID: {ProjectId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project with ID: {ProjectId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
    {
        try
        {
            var projects = await _projectService.GetAllProjectAsync();
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all projects");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Project>>> GetUserProjects(int userId)
    {
        try
        {
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects for user with ID: {UserId}", userId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Project>> UpdateProject(int id, [FromBody] ProjectUpdateDTO projectDto)
    {
        try
        {
            var project = await _projectService.UpdateProjectAsync(id, projectDto);
            return Ok(project);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project not found with ID: {ProjectId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project with ID: {ProjectId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project not found with ID: {ProjectId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project with ID: {ProjectId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{projectId}/users/{userId}")]
    public async Task<IActionResult> AddUserToProject(int projectId, int userId)
    {
        try
        {
            await _projectService.AddUserToProjectAsync(projectId, userId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project or user not found. ProjectId: {ProjectId}, UserId: {UserId}", projectId,
                userId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user to project. ProjectId: {ProjectId}, UserId: {UserId}", projectId,
                userId);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{projectId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromProject(int projectId, int userId)
    {
        try
        {
            await _projectService.RemoveUserFromProjectAsync(projectId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project or user not found. ProjectId: {ProjectId}, UserId: {UserId}", projectId,
                userId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from project. ProjectId: {ProjectId}, UserId: {UserId}",
                projectId, userId);
            return BadRequest(ex.Message);
        }
    }
}