using TaskManager.Api.Models.Domains.Domains;
using TaskManager.Api.Models.DTOs;

namespace TaskManager.Api.Services.Interfaces;

public interface IProjectService
{
    /// <summary>
    /// Создает новый проект асинхронно на основе предоставленных данных.
    /// </summary>
    /// <param name="projectDto">DTO с данными для создания проекта. Может быть null.</param>
    /// <returns>Задача, результатом которой является созданный проект.</returns>
    Task<Project> CreateProjectAsync(ProjectCreateDTO? projectDto);

    /// <summary>
    /// Получает проект по его идентификатору асинхронно.
    /// </summary>
    /// <param name="userId">Идентификатор проекта.</param>
    /// <returns>Задача, результатом которой является найденный проект.</returns>
    Task<Project> GetProjectByIdAsync(int userId);

    /// <summary>
    /// Получает все проекты асинхронно.
    /// </summary>
    /// <returns>Задача, результатом которой является коллекция всех проектов.</returns>
    Task<IEnumerable<Project>> GetAllProjectAsync();

    /// <summary>
    /// Обновляет проект асинхронно по его идентификатору на основе предоставленных данных.
    /// </summary>
    /// <param name="id">Идентификатор проекта, который нужно обновить.</param>
    /// <param name="projectDto">DTO с данными для обновления проекта.</param>
    /// <returns>Задача, результатом которой является обновленный проект.</returns>
    Task<Project> UpdateProjectAsync(int id, ProjectUpdateDTO projectDto);

    /// <summary>
    /// Удаляет проект асинхронно по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор проекта, который нужно удалить.</param>
    /// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
    Task DeleteProjectAsync(int id);

    /// <summary>
    /// Получает все проекты пользователя асинхронно.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Задача, результатом которой является коллекция всех проектов пользователя.</returns>
    Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);

    /// <summary>
    /// Добавляет пользователя к проекту асинхронно.
    /// </summary>
    /// <param name="projectId">Идентификатор проекта.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Задача, результатом которой является обновленный проект.</returns>
    Task<Project> AddUserToProjectAsync(int projectId, int userId);

    /// <summary>
    /// Удаляет пользователя из проекта асинхронно.
    /// </summary>
    /// <param name="projectId">Идентификатор проекта.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Задача, результатом которой является обновленный проект.</returns>
    Task<Project> RemoveUserFromProjectAsync(int projectId, int userId);
}