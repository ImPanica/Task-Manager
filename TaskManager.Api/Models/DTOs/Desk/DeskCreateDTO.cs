using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models.Domains.Domains;

namespace TaskManager.Api.Models.DTOs;

public class DeskCreateDTO
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Description { get; set; }

    [Required]
    public bool IsPrivate { get; set; }

    [Required]
    public int AdminId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public byte[]? Photo { get; set; }
}