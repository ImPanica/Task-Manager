using TaskManager.Api.Models.Domains;
using Task = TaskManager.Api.Models.Domains.Task;

namespace TaskManager.Api.Models.DTOs;

public class DeskReadDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPrivate { get; set; }
    public string Columns { get; set; }
    public DateTime CreateDateTime { get; set; }
    public byte[]? Photo { get; set; }
    
    public int? AdminId { get; set; }
    public string? AdminName { get; set; }
    
    public int? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    
    public ICollection<Task> Tasks { get; set; }
}