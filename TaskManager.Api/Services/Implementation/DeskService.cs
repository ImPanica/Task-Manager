using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Implementation;

public class DeskService : IDeskService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<DeskService> _logger;

    /// <summary>
    /// Конструктор сервиса досок
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="logger">Сервис логирования</param>
    public DeskService(
        ApplicationContext context,
        ILogger<DeskService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Desk> CreateDeskAsync(DeskCreateDTO? deskDto)
    {
        if (deskDto == null)
            throw new ArgumentNullException(nameof(deskDto));

        var desk = new Desk
        {
            Name = deskDto.Name,
            Description = deskDto.Description,
            IsPrivate = deskDto.IsPrivate,
            AdminId = deskDto.AdminId,
            ProjectId = deskDto.ProjectId,
            Photo = deskDto.Photo,
            CreateDateTime = DateTime.UtcNow
        };

        // Создаем стандартные колонки для доски
        var defaultColumns = new[]
        {
            new Column 
            { 
                Name = "To Do", 
                Order = 1, 
                Description = "Tasks that need to be done",
                Desk = desk
            },
            new Column 
            { 
                Name = "In Progress", 
                Order = 2, 
                Description = "Tasks currently being worked on",
                Desk = desk
            },
            new Column 
            { 
                Name = "Done", 
                Order = 3, 
                Description = "Completed tasks",
                Desk = desk
            }
        };

        desk.Columns = defaultColumns.ToList();

        await _context.Desks.AddAsync(desk);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new desk with ID: {DeskId} and default columns", desk.Id);
        return desk;
    }

    public async Task<Desk> GetDeskByIdAsync(int deskId)
    {
        var desk = await _context.Desks
            .Include(d => d.Admin)
            .Include(d => d.Project)
            .Include(d => d.Columns)
            .Include(d => d.Tasks)
                .ThenInclude(t => t.Creator)
            .Include(d => d.Tasks)
                .ThenInclude(t => t.Executor)
            .FirstOrDefaultAsync(d => d.Id == deskId);

        if (desk == null)
            throw new KeyNotFoundException($"Desk with ID {deskId} not found");

        return desk;
    }

    public async Task<IEnumerable<Desk>> GetAllDeskAsync()
    {
        return await _context.Desks
            .Include(d => d.Admin)
            .Include(d => d.Project)
            .Include(d => d.Columns)
            .Include(d => d.Tasks)
            .ToListAsync();
    }

    public async Task<Desk> UpdateDeskAsync(int id, DeskUpdateDTO deskDto)
    {
        if (deskDto == null)
            throw new ArgumentNullException(nameof(deskDto));

        var desk = await _context.Desks.FindAsync(id);
        if (desk == null)
            throw new KeyNotFoundException($"Desk with ID {id} not found");

        // Обновляем только те поля, которые были предоставлены в DTO
        if (deskDto.Name != null)
            desk.Name = deskDto.Name;
        if (deskDto.Description != null)
            desk.Description = deskDto.Description;
        if (deskDto.IsPrivate.HasValue)
            desk.IsPrivate = deskDto.IsPrivate.Value;
        if (deskDto.AdminId.HasValue)
            desk.AdminId = deskDto.AdminId;
        if (deskDto.ProjectId.HasValue)
            desk.ProjectId = deskDto.ProjectId;
        if (deskDto.Photo != null)
            desk.Photo = deskDto.Photo;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated desk with ID: {DeskId}", desk.Id);
        
        return desk;
    }

    public async Task DeleteDeskAsync(int id)
    {
        var desk = await _context.Desks.FindAsync(id);
        if (desk == null)
            throw new KeyNotFoundException($"Desk with ID {id} not found");

        _context.Desks.Remove(desk);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Deleted desk with ID: {DeskId}", id);
    }
}