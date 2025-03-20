using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

/// <summary>
/// Сервис для работы с пользователями: аутентификация, авторизация и управление учетными данными
/// </summary>
public class UserService
{
    private readonly ApplicationContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Конструктор сервиса пользователей
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="passwordHasher">Сервис для хеширования паролей</param>
    /// <param name="logger">Сервис логирования</param>
    public UserService(
        ApplicationContext context,
        IPasswordHasher<User> passwordHasher,
        ILogger<UserService> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger;
    }

    /// <summary>
    /// Извлекает логин и пароль из заголовка Basic Authentication
    /// </summary>
    /// <param name="request">HTTP запрос</param>
    /// <returns>Кортеж с логином и паролем пользователя</returns>
    public (string Username, string Password) GetUserCredentialsFromBasicAuth(HttpRequest request)
    {
        // Проверка наличия заголовка Authorization
        if (!request.Headers.TryGetValue("Authorization", out var authHeaderValue) ||
            string.IsNullOrEmpty(authHeaderValue))
        {
            _logger?.LogDebug("Отсутствует заголовок Authorization");
            return (string.Empty, string.Empty);
        }

        var authHeader = authHeaderValue.ToString();

        // Проверка, что это Basic Authentication
        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            _logger?.LogDebug("Неподдерживаемый тип аутентификации: {AuthType}", authHeader.Split(' ')[0]);
            return (string.Empty, string.Empty);
        }

        try
        {
            // Извлечение и декодировка данных
            var encodedCredentials = authHeader["Basic ".Length..].Trim();
            var credentialsBytes = Convert.FromBase64String(encodedCredentials);
            var credentials = Encoding.UTF8.GetString(credentialsBytes);

            // Разделение логина и пароля
            var separatorIndex = credentials.IndexOf(':');
            if (separatorIndex == -1)
            {
                _logger?.LogWarning("Неверный формат данных в Basic Authentication");
                return (string.Empty, string.Empty);
            }

            // Извлечение логина и пароля
            var username = credentials[..separatorIndex].Trim();
            var password = credentials[(separatorIndex + 1)..].Trim();

            return (username, password);
        }
        catch (Exception ex)
        {
            // Обработка ошибок декодирования
            _logger?.LogError(ex, "Ошибка при декодировании Basic Authentication");
            return (string.Empty, string.Empty);
        }
    }

    /// <summary>
    /// Аутентифицирует пользователя по логину и паролю
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Объект пользователя при успешной аутентификации, иначе null</returns>
    public User AuthenticateUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger?.LogDebug("Попытка аутентификации с пустым логином или паролем");
            return null;
        }

        try
        {
            // Находим пользователя по логину
            var user = _context.Users.SingleOrDefault(u => u.Login == username);

            if (user == null)
            {
                _logger?.LogDebug("Пользователь с логином {Username} не найден", username);
                return null;
            }

            // Проверяем пароль
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            // Возвращаем пользователя только при успешной верификации пароля
            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                _logger?.LogInformation("Успешная аутентификация пользователя {Username}", username);
                return user;
            }

            _logger?.LogWarning("Неверный пароль для пользователя {Username}", username);
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при аутентификации пользователя {Username}", username);
            throw new ApplicationException("Ошибка аутентификации", ex);
        }
    }

    /// <summary>
    /// Создает объект ClaimsIdentity для аутентифицированного пользователя
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Объект ClaimsIdentity при успешной аутентификации, иначе null</returns>
    public ClaimsIdentity GetUserClaimsIdentity(string username, string password)
    {
        // Аутентифицируем пользователя
        var user = AuthenticateUser(username, password);

        // Если пользователь не аутентифицирован, возвращаем null
        if (user == null)
        {
            return null;
        }

        try
        {
            // Обновляем дату последнего входа
            user.LastLoginDate = DateTime.Now;
            _context.Users.Update(user);
            _context.SaveChanges();

            // Создаем список утверждений (claims) для пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimTypes.Role, user.UserStatus.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Создаем ClaimsIdentity для JWT-токена
            var identity = new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return identity;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при создании ClaimsIdentity для пользователя {Username}", username);
            throw new ApplicationException("Ошибка создания токена", ex);
        }
    }
}