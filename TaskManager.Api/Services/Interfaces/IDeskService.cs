using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Interfaces;

public interface IDeskService
{
    /// <summary>
    /// Создает новую доску (Desk) асинхронно на основе предоставленных данных.
    /// </summary>
    /// <param name="deskDto">DTO с данными для создания доски. Может быть null.</param>
    /// <returns>Задача, результатом которой является созданная доска.</returns>
    Task<Desk> CreateDeskAsync(DeskCreateDTO? deskDto);

    /// <summary>
    /// Получает доску (Desk) по её идентификатору асинхронно.
    /// </summary>
    /// <param name="deskId">Идентификатор доски.</param>
    /// <returns>Задача, результатом которой является найденная доска.</returns>
    Task<Desk> GetDeskByIdAsync(int deskId);

    /// <summary>
    /// Получает все доски (Desk) асинхронно.
    /// </summary>
    /// <returns>Задача, результатом которой является коллекция всех досок.</returns>
    Task<IEnumerable<Desk>> GetAllDeskAsync();

    /// <summary>
    /// Обновляет доску (Desk) асинхронно по её идентификатору на основе предоставленных данных.
    /// </summary>
    /// <param name="id">Идентификатор доски, которую нужно обновить.</param>
    /// <param name="deskDto">DTO с данными для обновления доски.</param>
    /// <returns>Задача, результатом которой является обновленная доска.</returns>
    Task<Desk> UpdateDeskAsync(int id, DeskUpdateDTO deskDto);

    /// <summary>
    /// Удаляет доску (Desk) асинхронно по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор доски, которую нужно удалить.</param>
    /// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
    Task DeleteDeskAsync(int id);
}