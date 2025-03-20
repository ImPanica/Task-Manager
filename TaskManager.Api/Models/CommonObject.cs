using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models;

public class CommonObject
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    [StringLength(100)]
    public string Description { get; set; }
    public DateTime CreateDateTime { get; set; } = DateTime.Now;
    public byte[]? Photo { get; set; }
}