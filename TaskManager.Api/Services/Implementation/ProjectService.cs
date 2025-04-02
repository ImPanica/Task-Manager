using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Implementation;

public class ProjectService : IProjectService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<ProjectService> _logger;

    /// <summary>
    /// Конструктор сервиса проектов
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="logger">Сервис логирования</param>
    public ProjectService(
        ApplicationContext context,
        ILogger<ProjectService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Project> CreateProjectAsync(ProjectCreateDTO? projectDto)
    {
        if (projectDto == null)
            throw new ArgumentNullException(nameof(projectDto));

        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description,
            AdminId = projectDto.AdminId,
            ProjectStatus = projectDto.ProjectStatus,
            Photo = projectDto.Photo,
            CreateDateTime = DateTime.UtcNow
        };

        // Добавляем пользователей в проект
        if (projectDto.UserIds != null && projectDto.UserIds.Any())
        {
            var users = await _context.Users
                .Where(u => projectDto.UserIds.Contains(u.Id))
                .ToListAsync();
            project.AllUsers = users;
        }

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new project with ID: {ProjectId}", project.Id);
        return project;
    }

    public async Task<Project> GetProjectByIdAsync(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Admin)
                .ThenInclude(a => a.User)
            .Include(p => p.AllUsers)
            .Include(p => p.AllDesks)
                .ThenInclude(d => d.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new KeyNotFoundException($"Project with ID {projectId} not found");

        return project;
    }

    public async Task<IEnumerable<Project>> GetAllProjectAsync()
    {
        return await _context.Projects
            .Include(p => p.Admin)
                .ThenInclude(a => a.User)
            .Include(p => p.AllUsers)
            .Include(p => p.AllDesks)
            .ToListAsync();
    }

    public async Task<Project> UpdateProjectAsync(int id, ProjectUpdateDTO projectDto)
    {
        if (projectDto == null)
            throw new ArgumentNullException(nameof(projectDto));

        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            throw new KeyNotFoundException($"Project with ID {id} not found");

        // Обновляем только те поля, которые были предоставлены в DTO
        if (projectDto.Name != null)
            project.Name = projectDto.Name;
        if (projectDto.Description != null)
            project.Description = projectDto.Description;
        if (projectDto.AdminId.HasValue)
            project.AdminId = projectDto.AdminId;
        if (projectDto.ProjectStatus.HasValue)
            project.ProjectStatus = projectDto.ProjectStatus.Value;
        if (projectDto.Photo != null)
            project.Photo = projectDto.Photo;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated project with ID: {ProjectId}", project.Id);
        
        return project;
    }

    public async Task DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            throw new KeyNotFoundException($"Project with ID {id} not found");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Deleted project with ID: {ProjectId}", id);
    }

    public async Task<Project> AddUserToProjectAsync(int projectId, int userId)
    {
        var project = await _context.Projects
            .Include(p => p.AllUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        
        if (project == null)
            throw new KeyNotFoundException($"Project with ID {projectId} not found");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        if (!project.AllUsers.Any(u => u.Id == userId))
        {
            project.AllUsers.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added user {UserId} to project {ProjectId}", userId, projectId);
        }

        return project;
    }

    public async Task<Project> RemoveUserFromProjectAsync(int projectId, int userId)
    {
        var project = await _context.Projects
            .Include(p => p.AllUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        
        if (project == null)
            throw new KeyNotFoundException($"Project with ID {projectId} not found");

        var user = project.AllUsers.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            project.AllUsers.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Removed user {UserId} from project {ProjectId}", userId, projectId);
        }

        return project;
    }

    public async Task<IEnumerable<Project>> GetUserProjectsAsync(int userId)
    {
        return await _context.Projects
            .Include(p => p.Admin)
                .ThenInclude(a => a.User)
            .Include(p => p.AllUsers)
            .Where(p => p.AllUsers.Any(u => u.Id == userId))
            .ToListAsync();
    }
}