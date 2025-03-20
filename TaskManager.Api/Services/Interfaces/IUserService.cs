using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Создает нового пользователя асинхронно на основе предоставленных данных.
    /// </summary>
    /// <param name="userDto">DTO (Data Transfer Object) с данными для создания пользователя. Может быть null.</param>
    /// <returns>Задача, результатом которой является созданный пользователь.</returns>
    Task<User> CreateUserAsync(UserCreateDTO? userDto);

    /// <summary>
    /// Создает несколько пользователей асинхронно на основе предоставленного списка данных.
    /// </summary>
    /// <param name="usersDto">Список DTO (Data Transfer Object) с данными для создания пользователей. Может быть null.</param>
    /// <returns>Задача, результатом которой является список созданных пользователей.</returns>
    Task<IEnumerable<User>> CreateUsersAsync(IEnumerable<UserCreateDTO>? usersDto);

    /// <summary>
    /// Получает пользователя по его идентификатору асинхронно.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Задача, результатом которой является пользователь с указанным идентификатором.</returns>
    Task<User> GetUserByIdAsync(int userId);

    /// <summary>
    /// Получает всех пользователей асинхронно.
    /// </summary>
    /// <returns>Задача, результатом которой является список всех пользователей.</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Обновляет данные пользователя асинхронно по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, данные которого нужно обновить.</param>
    /// <param name="userDTO">DTO (Data Transfer Object) с обновленными данными пользователя.</param>
    /// <returns>Задача, результатом которой является обновленный пользователь.</returns>
    Task<User> UpdateUserAsync(int id, UserUpdateDTO userDTO);

    /// <summary>
    /// Удаляет пользователя асинхронно по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого нужно удалить.</param>
    /// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
    Task DeleteUserAsync(int id);
}