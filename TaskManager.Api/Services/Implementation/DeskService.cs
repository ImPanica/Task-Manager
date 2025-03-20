using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.DTOs;
using TaskManager.Api.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Api.Services.Implementation;

public class DeskService : IDeskService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Конструктор сервиса досок
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="logger">Сервис логирования</param>
    public DeskService(
        ApplicationContext context,
        ILogger<UserService> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    public Task<Desk> CreateDeskAsync(DeskCreateDTO? deskDto)
    {
        throw new NotImplementedException();
    }

    public Task<Desk> GetDeskByIdAsync(int deskId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Desk>> GetAllDeskAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Desk> UpdateDeskAsync(int id, DeskUpdateDTO deskDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDeskAsync(int id)
    {
        throw new NotImplementedException();
    }
}