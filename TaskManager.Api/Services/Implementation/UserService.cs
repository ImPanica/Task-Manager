using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using TaskManager.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Implementation;

/// <summary>
/// Сервис для работы с пользователями: аутентификация, авторизация и управление учетными данными
/// </summary>
public class UserService : IUserService
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Добавляем роль в зависимости от статуса пользователя
            if (user.UserStatus == UserStatus.Admin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            // Создаем ClaimsIdentity для JWT-токена
            var identity = new ClaimsIdentity(
                claims,
                "jwt",
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

    /// <summary>
    /// Создает нового пользователя из DTO
    /// </summary>
    public async Task<User> CreateUserAsync(UserCreateDTO? userDto)
    {
        if (userDto == null)
            throw new ArgumentNullException(nameof(userDto));

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Login = userDto.Login,
            Email = userDto.Email,
            Phone = userDto.Phone,
            Photo = userDto.Photo,
            UserStatus = userDto.UserStatus,
            RegistrationDate = DateTime.Now,
            LastLoginDate = DateTime.Now
        };

        // Хэшируем пароль
        user.Password = _passwordHasher.HashPassword(user, userDto.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Создает несколько пользователей из списка DTO
    /// </summary>
    public async Task<IEnumerable<User>> CreateUsersAsync(IEnumerable<UserCreateDTO>? usersDto)
    {
        if (usersDto == null)
            throw new ArgumentNullException(nameof(usersDto));

        var users = new List<User>();
        foreach (var userDto in usersDto)
        {
            var user = await CreateUserAsync(userDto);
            users.Add(user);
        }

        return users;
    }

    /// <summary>
    /// Получает пользователя по ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    /// <summary>
    /// Получает всех пользователей
    /// </summary>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    /// <summary>
    /// Обновляет данные пользователя
    /// </summary>
    public async Task<User> UpdateUserAsync(int id, UserUpdateDTO userDTO)
    {
        var user = await GetUserByIdAsync(id);

        // Обновляем только предоставленные поля
        user.FirstName = userDTO.FirstName;
        user.LastName = userDTO.LastName;
        user.Login = userDTO.Login;
        user.Email = userDTO.Email;
        user.Phone = userDTO.Phone ?? user.Phone;
        user.Photo = userDTO.Photo ?? user.Photo;
        user.UserStatus = userDTO.UserStatus ?? user.UserStatus;

        // Обновляем пароль только если он был предоставлен
        if (!string.IsNullOrEmpty(userDTO.Password))
        {
            user.Password = _passwordHasher.HashPassword(user, userDTO.Password);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    public async Task DeleteUserAsync(int id)
    {
        var user = await GetUserByIdAsync(id);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Создает ClaimsIdentity для JWT-токена
    /// </summary>
    public ClaimsIdentity CreateClaimsIdentity(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _logger?.LogInformation("Creating claims for user {Username} with status {UserStatus}", user.Login, user.UserStatus);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        // Add role claim based on user status
        switch (user.UserStatus)
        {
            case UserStatus.Admin:
                _logger?.LogInformation("Adding Admin role claim for user {Username}", user.Login);
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                break;
            case UserStatus.Editor:
                _logger?.LogInformation("Adding Editor role claim for user {Username}", user.Login);
                claims.Add(new Claim(ClaimTypes.Role, "Editor"));
                break;
            default:
                _logger?.LogInformation("Adding User role claim for user {Username}", user.Login);
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                break;
        }

        return new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
    }
}