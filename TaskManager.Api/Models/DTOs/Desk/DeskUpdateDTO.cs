using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models.DTOs;

public class DeskUpdateDTO
{
    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }

    public bool? IsPrivate { get; set; }

    public string? Columns { get; set; }

    public int? AdminId { get; set; }

    public int? ProjectId { get; set; }

    public byte[]? Photo { get; set; }
}