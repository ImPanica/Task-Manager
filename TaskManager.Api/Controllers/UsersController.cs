using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Implementation;

namespace TaskManager.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UserService> _logger;

    public UsersController(UserService userService, ILogger<UserService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Создает нового пользователя
    /// </summary>
    [HttpPost("create")]
    public async Task<ActionResult<UserReadDTO>> CreateUser([FromBody] UserCreateDTO? userDto)
    {
        if (userDto == null)
            return BadRequest("User model cannot be null");

        try
        {
            var user = await _userService.CreateUserAsync(userDto);
            var userReadDto = UserReadDTO.MapToUserReadDTO(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userReadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка создания нового пользователя");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Создает несколько пользователей
    /// </summary>
    [HttpPost("create/bulk")]
    public async Task<ActionResult<IEnumerable<UserReadDTO>>> CreateUsers([FromBody] IEnumerable<UserCreateDTO>? usersDto)
    {
        if (usersDto == null)
            return BadRequest("User models cannot be null");

        try
        {
            var users = await _userService.CreateUsersAsync(usersDto);
            var userReadDtos = users.Select(UserReadDTO.MapToUserReadDTO);
            return CreatedAtAction(nameof(GetAllUsers), null, userReadDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка создания пользователей");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получает пользователя по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDTO>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            var userReadDto = UserReadDTO.MapToUserReadDTO(user);
            return Ok(userReadDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения пользователя");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Обновляет данные пользователя
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserReadDTO>> UpdateUser(int id, [FromBody] UserUpdateDTO? userDto)
    {
        if (userDto == null)
            return BadRequest("User model cannot be null");

        try
        {
            var user = await _userService.UpdateUserAsync(id, userDto);
            var userReadDto = UserReadDTO.MapToUserReadDTO(user);
            return Ok(userReadDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обновления пользователя");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка удаления пользователя");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получает всех пользователей
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UserReadDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var userReadDtos = users.Select(UserReadDTO.MapToUserReadDTO);
            return Ok(userReadDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения списка пользователей");
            return StatusCode(500, "Internal server error");
        }
    }
}