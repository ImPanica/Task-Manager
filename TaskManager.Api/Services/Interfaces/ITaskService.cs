using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Interfaces;

public interface ITaskService
{
    /// <summary>
    /// Создает новую задачу асинхронно
    /// </summary>
    /// <param name="taskDto">DTO с данными для создания задачи</param>
    /// <returns>Созданная задача</returns>
    Task<Models.Domains.Task> CreateTaskAsync(TaskCreateDTO taskDto);

    /// <summary>
    /// Получает задачу по ID асинхронно
    /// </summary>
    /// <param name="taskId">ID задачи</param>
    /// <returns>Найденная задача</returns>
    Task<Models.Domains.Task> GetTaskByIdAsync(int taskId);

    /// <summary>
    /// Получает все задачи асинхронно
    /// </summary>
    /// <returns>Коллекция всех задач</returns>
    Task<IEnumerable<Models.Domains.Task>> GetAllTasksAsync();

    /// <summary>
    /// Получает все задачи для конкретной доски асинхронно
    /// </summary>
    /// <param name="deskId">ID доски</param>
    /// <returns>Коллекция задач для доски</returns>
    Task<IEnumerable<Models.Domains.Task>> GetTasksByDeskIdAsync(int deskId);

    /// <summary>
    /// Получает все задачи для конкретной колонки асинхронно
    /// </summary>
    /// <param name="columnId">ID колонки</param>
    /// <returns>Коллекция задач для колонки</returns>
    Task<IEnumerable<Models.Domains.Task>> GetTasksByColumnIdAsync(int columnId);

    /// <summary>
    /// Обновляет существующую задачу асинхронно
    /// </summary>
    /// <param name="taskId">ID задачи</param>
    /// <param name="taskDto">DTO с данными для обновления</param>
    /// <returns>Обновленная задача</returns>
    Task<Models.Domains.Task> UpdateTaskAsync(int taskId, TaskUpdateDTO taskDto);

    /// <summary>
    /// Удаляет задачу асинхронно
    /// </summary>
    /// <param name="taskId">ID задачи</param>
    Task DeleteTaskAsync(int taskId);

    /// <summary>
    /// Перемещает задачу в другую колонку асинхронно
    /// </summary>
    /// <param name="taskId">ID задачи</param>
    /// <param name="newColumnId">ID новой колонки</param>
    /// <returns>Обновленная задача</returns>
    Task<Models.Domains.Task> MoveTaskToColumnAsync(int taskId, int newColumnId);

    /// <summary>
    /// Назначает исполнителя задачи асинхронно
    /// </summary>
    /// <param name="taskId">ID задачи</param>
    /// <param name="executorId">ID исполнителя</param>
    /// <returns>Обновленная задача</returns>
    Task<Models.Domains.Task> AssignExecutorAsync(int taskId, int executorId);

    /// <summary>
    /// Получает задачи для текущего пользователя с учетом его роли
    /// </summary>
    /// <param name="userId">ID текущего пользователя</param>
    /// <param name="userStatus">Статус пользователя</param>
    /// <returns>Коллекция задач</returns>
    Task<IEnumerable<Models.Domains.Task>> GetTasksForCurrentUserAsync(int userId, UserStatus userStatus);
}