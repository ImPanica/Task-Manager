using Microsoft.AspNetCore.Identity;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.Domains.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Implementation;

public class ProjectService : IProjectService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Конструктор сервиса проектов
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="logger">Сервис логирования</param>
    public ProjectService(
        ApplicationContext context,
        ILogger<UserService> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    public async Task<Project> CreateProjectAsync(ProjectCreateDTO? projectDto)
    {
        throw new NotImplementedException();
    }

    public async Task<Project> GetProjectByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Project>> GetAllProjectAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Project> UpdateProjectAsync(int id, ProjectUpdateDTO projectDto)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteProjectAsync(int id)
    {
        throw new NotImplementedException();
    }
}