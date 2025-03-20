using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using TaskManager.Models;
using Microsoft.AspNetCore.Identity;

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

    public AccountController(ApplicationContext context, IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ILogger<UserService> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger;
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userService = new UserService(_context, _passwordHasher, _logger);
    }

    [HttpGet("info")]
    public async Task<ActionResult<User>> GetUser()
    {
        var userName = HttpContext.User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Login == userName);

        if (user == null)
            return NotFound("User not found");

        return user;
    }

    /// <summary>
    /// Аутентификация пользователя и выдача JWT токена
    /// </summary>
    /// <returns>JWT токен в случае успешной аутентификации</returns>
    [HttpPost("auth")]
    public async Task<IActionResult> GetToken()
    {
        try
        {
            _logger.LogInformation("Попытка получить учетные данные из Basic Auth");
            // Получаем учетные данные из Basic Auth заголовка
            var (username, password) = _userService.GetUserCredentialsFromBasicAuth(Request);
            _logger.LogInformation($"Получены данные: username={username != null}, password={password != null}");

            // Проверяем наличие учетных данных
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { error = "Отсутствуют учетные данные" });
            }

            // Получаем ClaimsIdentity для пользователя
            var identity = _userService.GetUserClaimsIdentity(username, password);

            // Если аутентификация не удалась, возвращаем ошибку
            if (identity == null)
            {
                return Unauthorized(new { error = "Неверный логин или пароль" });
            }

            // Создаем и настраиваем токен
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"]))),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
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
            _logger.LogError(ex, "Ошибка при генерации JWT токена");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Метод для обновления данных пользователя по ID
    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserModel? userModel)
    {
        if (userModel == null)
            return BadRequest("User Model cannot be null");

        // Поиск пользователя по Login
        var login = HttpContext.User.Identity.Name;
        var user = await _context.Users.FindAsync(login);
        if (user == null)
            return NotFound($"User Not Found");

        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.Login = userModel.Login;
        user.Email = userModel.Email;
        user.Password = _passwordHasher.HashPassword(user, userModel.Password);
        user.Phone = userModel.Phone;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        // Возвращаем результат и новые данные пользователя
        return Ok(user);
    }
}