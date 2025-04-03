using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using TaskManager.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Implementation;

public class TaskService : ITaskService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ApplicationContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Models.Domains.Task> CreateTaskAsync(TaskCreateDTO taskDto)
    {
        try
        {
            var task = new Models.Domains.Task
            {
                Name = taskDto.Name,
                Description = taskDto.Description,
                StartDate = taskDto.StartDate,
                EndDate = taskDto.EndDate,
                File = taskDto.File,
                Photo = taskDto.Photo,
                DeskId = taskDto.DeskId,
                ColumnId = taskDto.ColumnId,
                CreatorId = taskDto.CreatorId,
                ExecutorId = taskDto.ExecutorId,
                CreateDateTime = DateTime.Now
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new task with ID: {TaskId}", task.Id);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            throw;
        }
    }

    public async Task<Models.Domains.Task> GetTaskByIdAsync(int taskId)
    {
        try
        {
            var task = await _context.Tasks
                .Include(t => t.Desk)
                .Include(t => t.Column)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found", taskId);
                throw new KeyNotFoundException($"Task with ID {taskId} not found");
            }

            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task with ID: {TaskId}", taskId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.Domains.Task>> GetAllTasksAsync()
    {
        try
        {
            return await _context.Tasks
                .Include(t => t.Desk)
                .Include(t => t.Column)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            throw;
        }
    }

    public async Task<IEnumerable<Models.Domains.Task>> GetTasksByDeskIdAsync(int deskId)
    {
        try
        {
            return await _context.Tasks
                .Include(t => t.Desk)
                .Include(t => t.Column)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .Where(t => t.DeskId == deskId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for desk ID: {DeskId}", deskId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.Domains.Task>> GetTasksByColumnIdAsync(int columnId)
    {
        try
        {
            return await _context.Tasks
                .Include(t => t.Desk)
                .Include(t => t.Column)
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .Where(t => t.ColumnId == columnId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for column ID: {ColumnId}", columnId);
            throw;
        }
    }

    public async Task<Models.Domains.Task> UpdateTaskAsync(int taskId, TaskUpdateDTO taskDto)
    {
        try
        {
            var task = await GetTaskByIdAsync(taskId);

            if (taskDto.Name != null)
                task.Name = taskDto.Name;
            if (taskDto.Description != null)
                task.Description = taskDto.Description;
            if (taskDto.StartDate.HasValue)
                task.StartDate = taskDto.StartDate.Value;
            if (taskDto.EndDate.HasValue)
                task.EndDate = taskDto.EndDate.Value;
            if (taskDto.File != null)
                task.File = taskDto.File;
            if (taskDto.Photo != null)
                task.Photo = taskDto.Photo;
            if (taskDto.DeskId.HasValue)
                task.DeskId = taskDto.DeskId.Value;
            if (taskDto.ColumnId.HasValue)
                task.ColumnId = taskDto.ColumnId.Value;
            if (taskDto.ExecutorId.HasValue)
                task.ExecutorId = taskDto.ExecutorId.Value;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated task with ID: {TaskId}", taskId);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task with ID: {TaskId}", taskId);
            throw;
        }
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        try
        {
            var task = await GetTaskByIdAsync(taskId);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted task with ID: {TaskId}", taskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task with ID: {TaskId}", taskId);
            throw;
        }
    }

    public async Task<Models.Domains.Task> MoveTaskToColumnAsync(int taskId, int newColumnId)
    {
        try
        {
            var task = await GetTaskByIdAsync(taskId);
            task.ColumnId = newColumnId;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Moved task {TaskId} to column {ColumnId}", taskId, newColumnId);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving task {TaskId} to column {ColumnId}", taskId, newColumnId);
            throw;
        }
    }

    public async Task<Models.Domains.Task> AssignExecutorAsync(int taskId, int executorId)
    {
        try
        {
            var task = await GetTaskByIdAsync(taskId);
            task.ExecutorId = executorId;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Assigned executor {ExecutorId} to task {TaskId}", executorId, taskId);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning executor {ExecutorId} to task {TaskId}", executorId, taskId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.Domains.Task>> GetTasksForCurrentUserAsync(int userId, UserStatus userStatus)
    {
        try
        {
            var query = _context.Tasks
                .Include(t => t.Desk)
                .Include(t => t.Column)
                .Include(t => t.Creator)
                .Include(t => t.Executor);

            // Если пользователь Admin или Editor, возвращаем все задачи
            if (userStatus == UserStatus.Admin || userStatus == UserStatus.Editor)
            {
                _logger.LogInformation("User {UserId} with status {UserStatus} requesting all tasks", userId, userStatus);
                return await query.ToListAsync();
            }

            // Для обычных пользователей возвращаем только назначенные им задачи
            _logger.LogInformation("User {UserId} with status {UserStatus} requesting assigned tasks", userId, userStatus);
            return await query.Where(t => t.ExecutorId == userId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks for user {UserId} with status {UserStatus}", userId, userStatus);
            throw;
        }
    }
}