using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Models;

namespace TaskManager.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UsersController(ApplicationContext context, PasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // Метод для создания нового пользователя
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserModel? userModel)
    {
        if (userModel == null)
            return BadRequest("User Model cannot be null");

        // Создаем объект с новым пользователем
        var user = new User(userModel);

        // Хэшируем пароль
        user.Password = _passwordHasher.HashPassword(user, userModel.Password);
        // Добавляем юзера
        _context.Users.Add(user);
        // Созраняем изменения
        await _context.SaveChangesAsync();
        // Возвращаем результат и ID созданного пользователя
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Метод для создания пользователей из списка
    [HttpPost("create/bulk")]
    public async Task<IActionResult> CreateUsers([FromBody] IEnumerable<UserModel>? users)
    {
        if (users == null)
            return BadRequest("User Model cannot be null");

        var newUsers = new List<User>();
        foreach (var user in users)
        {
            var newUser = new User(user);
            newUser.Password = _passwordHasher.HashPassword(newUser, user.Password);
            newUsers.Add(newUser);
        }

        _context.Users.AddRange(newUsers);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllUsers), null, newUsers);
    }

    // Метод для получения пользователя по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        // Поиск пользователя по ID
        var user = await _context.Users.FindAsync(id);
        // Возврат результата
        return user != null ? Ok(user) : NotFound($"User with id {id} Not Found");
    }

    // Метод для обновления данных пользователя по ID
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel? userModel)
    {
        if (userModel == null)
            return BadRequest("User Model cannot be null");

        // Поиск пользователя по ID
        var user = await _context.Users.FindAsync(id);
        if (!user.Id.Equals(id))
            return NotFound($"User with id {id} Not Found");

        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.Login = userModel.Login;
        user.Email = userModel.Email;
        user.Password = _passwordHasher.HashPassword(user, userModel.Password);
        user.Phone = userModel.Phone;
        user.UserStatus = userModel.UserStatus;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        // Возвращаем результат и новые данные пользователя
        return Ok(user);
    }

    // Метод для удаления пользователя по ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // Поиск пользователя по ID
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound($"User with id {id} Not Found");

        // Удаляем пользователя
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Метод для получения всех пользователей
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
}