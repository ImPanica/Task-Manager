using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Implementation;
using TaskManager.Models;

namespace TaskManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly UserService _userService;
    private readonly ILogger<UserService> _logger;

    public AccountController(
        ApplicationContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        UserService userService,
        ILogger<UserService> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger;
    }

    /// <summary>
    /// Получает информацию о текущем пользователе
    /// </summary>
    [HttpGet("info")]
    public async Task<ActionResult<UserReadDTO>> GetUser()
    {
        try
        {
            var userName = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User not authenticated");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userName);
            if (user == null)
                return NotFound("User not found");

            var userReadDto = UserReadDTO.MapToUserReadDTO(user);
            return Ok(userReadDto);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка получения информации о пользователе");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Аутентифицирует пользователя и возвращает JWT токен
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == loginDto.Login);
            if (user == null)
                return Unauthorized("Invalid login or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid login or password");

            // Обновляем дату последнего входа
            user.LastLoginDate = DateTime.Now;
            await _context.SaveChangesAsync();

            // Создаем ClaimsIdentity для JWT-токена
            var identity = _userService.CreateClaimsIdentity(user);

            // Создаем и настраиваем токен
            var now = DateTime.UtcNow;
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));
            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"]))),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256));

            // Сериализуем токен в строку
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Формируем ответ
            var response = new
            {
                access_token = encodedToken,
                username = identity.Name,
                expires_in = _configuration["Jwt:ExpiresMinutes"]
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при аутентификации пользователя");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Обновляет данные текущего пользователя
    /// </summary>
    [Authorize]
    [HttpPut("update")]
    public async Task<ActionResult<UserReadDTO>> UpdateUserAsync([FromBody] UserUpdateDTO? userDto)
    {
        if (userDto == null)
            return BadRequest("User model cannot be null");

        try
        {
            var userName = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User not authenticated");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userName);
            if (user == null)
                return NotFound("User not found");

            var updatedUser = await _userService.UpdateUserAsync(user.Id, userDto);
            var userReadDto = UserReadDTO.MapToUserReadDTO(updatedUser);
            return Ok(userReadDto);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка обновления данных пользователя");
            return StatusCode(500, "Internal server error");
        }
    }

}