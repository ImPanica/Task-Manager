using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models.DTOs;

public class TaskUpdateDTO
{
    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte[]? File { get; set; }
    public byte[]? Photo { get; set; }

    public int? DeskId { get; set; }
    public int? ColumnId { get; set; }
    public int? ExecutorId { get; set; }
}