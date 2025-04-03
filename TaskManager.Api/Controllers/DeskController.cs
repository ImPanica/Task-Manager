using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.Domains.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeskController : ControllerBase
{
    private readonly IDeskService _deskService;
    private readonly ILogger<DeskController> _logger;

    public DeskController(
        IDeskService deskService,
        ILogger<DeskController> logger)
    {
        _deskService = deskService ?? throw new ArgumentNullException(nameof(deskService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("create")]
    public async Task<ActionResult<Desk>> CreateDesk([FromBody] DeskCreateDTO deskDto)
    {
        try
        {
            var desk = await _deskService.CreateDeskAsync(deskDto);
            return CreatedAtAction(nameof(GetDesk), new { id = desk.Id }, desk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating desk");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Desk>> GetDesk(int id)
    {
        try
        {
            var desk = await _deskService.GetDeskByIdAsync(id);
            return Ok(desk);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Desk not found with ID: {DeskId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving desk with ID: {DeskId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Desk>>> GetAllDesks()
    {
        try
        {
            var desks = await _deskService.GetAllDeskAsync();
            return Ok(desks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all desks");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Desk>> UpdateDesk(int id, [FromBody] DeskUpdateDTO deskDto)
    {
        try
        {
            var desk = await _deskService.UpdateDeskAsync(id, deskDto);
            return Ok(desk);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Desk not found with ID: {DeskId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating desk with ID: {DeskId}", id);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDesk(int id)
    {
        try
        {
            await _deskService.DeleteDeskAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Desk not found with ID: {DeskId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting desk with ID: {DeskId}", id);
            return BadRequest(ex.Message);
        }
    }
}