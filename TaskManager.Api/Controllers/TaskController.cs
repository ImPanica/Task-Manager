using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using Task = System.Threading.Tasks.Task;
using System.Security.Claims;
using TaskManager.Models;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Создает новую задачу
    /// </summary>
    /// <param name="taskDto">Данные для создания задачи</param>
    /// <returns>Созданная задача</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDTO taskDto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(taskDto);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, "An error occurred while creating the task");
        }
    }

    /// <summary>
    /// Получает задачу по ID
    /// </summary>
    /// <param name="id">ID задачи</param>
    /// <returns>Задача</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task with ID: {TaskId}", id);
            return StatusCode(500, "An error occurred while getting the task");
        }
    }

    /// <summary>
    /// Получает все задачи
    /// </summary>
    /// <returns>Список всех задач</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            return StatusCode(500, "An error occurred while getting tasks");
        }
    }

    /// <summary>
    /// Получает все задачи для конкретной доски
    /// </summary>
    /// <param name="deskId">ID доски</param>
    /// <returns>Список задач для доски</returns>
    [HttpGet("desk/{deskId}")]
    public async Task<IActionResult> GetTasksByDesk(int deskId)
    {
        try
        {
            var tasks = await _taskService.GetTasksByDeskIdAsync(deskId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for desk ID: {DeskId}", deskId);
            return StatusCode(500, "An error occurred while getting tasks for the desk");
        }
    }

    /// <summary>
    /// Получает все задачи для конкретной колонки
    /// </summary>
    /// <param name="columnId">ID колонки</param>
    /// <returns>Список задач для колонки</returns>
    [HttpGet("column/{columnId}")]
    public async Task<IActionResult> GetTasksByColumn(int columnId)
    {
        try
        {
            var tasks = await _taskService.GetTasksByColumnIdAsync(columnId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for column ID: {ColumnId}", columnId);
            return StatusCode(500, "An error occurred while getting tasks for the column");
        }
    }

    /// <summary>
    /// Обновляет существующую задачу
    /// </summary>
    /// <param name="id">ID задачи</param>
    /// <param name="taskDto">Данные для обновления</param>
    /// <returns>Обновленная задача</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDTO taskDto)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(id, taskDto);
            return Ok(task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task with ID: {TaskId}", id);
            return StatusCode(500, "An error occurred while updating the task");
        }
    }

    /// <summary>
    /// Удаляет задачу
    /// </summary>
    /// <param name="id">ID задачи</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
            return StatusCode(500, "An error occurred while deleting the task");
        }
    }

    /// <summary>
    /// Перемещает задачу в другую колонку
    /// </summary>
    /// <param name="id">ID задачи</param>
    /// <param name="newColumnId">ID новой колонки</param>
    /// <returns>Обновленная задача</returns>
    [HttpPut("{id}/move")]
    public async Task<IActionResult> MoveTask(int id, [FromQuery] int newColumnId)
    {
        try
        {
            var task = await _taskService.MoveTaskToColumnAsync(id, newColumnId);
            return Ok(task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving task {TaskId} to column {ColumnId}", id, newColumnId);
            return StatusCode(500, "An error occurred while moving the task");
        }
    }

    /// <summary>
    /// Назначает исполнителя задачи
    /// </summary>
    /// <param name="id">ID задачи</param>
    /// <param name="executorId">ID исполнителя</param>
    /// <returns>Обновленная задача</returns>
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignExecutor(int id, [FromQuery] int executorId)
    {
        try
        {
            var task = await _taskService.AssignExecutorAsync(id, executorId);
            return Ok(task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Task with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning executor {ExecutorId} to task {TaskId}", executorId, id);
            return StatusCode(500, "An error occurred while assigning the executor");
        }
    }

    /// <summary>
    /// Получает задачи для текущего пользователя с учетом его роли
    /// </summary>
    /// <returns>Список задач</returns>
    [HttpGet("my-tasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        try
        {
            // Получаем ID и статус текущего пользователя из claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userStatusClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || userStatusClaim == null)
            {
                return Unauthorized("User information not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);
            var userStatus = userStatusClaim.Value switch
            {
                "Admin" => UserStatus.Admin,
                "Editor" => UserStatus.Editor,
                _ => UserStatus.User
            };

            var tasks = await _taskService.GetTasksForCurrentUserAsync(userId, userStatus);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for current user");
            return StatusCode(500, "An error occurred while getting tasks");
        }
    }
}