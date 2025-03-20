using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models.Domains;

namespace TaskManager.Api.Models.DTOs;

public class ProjectCreateDTO
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Description { get; set; }
    
    public byte[]? Photo { get; set; }
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.InProgress;
    
    public int? AdminId { get; set; }
    public ICollection<int> UserIds { get; set; } = new List<int>();
}