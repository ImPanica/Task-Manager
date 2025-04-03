using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models.DTOs;

public class TaskCreateDTO
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public byte[]? File { get; set; }
    public byte[]? Photo { get; set; }

    [Required]
    public int DeskId { get; set; }

    [Required]
    public int ColumnId { get; set; }

    public int? CreatorId { get; set; }
    public int? ExecutorId { get; set; }
}