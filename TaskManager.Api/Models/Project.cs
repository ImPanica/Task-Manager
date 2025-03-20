using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Models;

public class Project : CommonObject
{
    [Key]
    public int Id { get; set; }
    public int? AdminId { get; set; }
    [ForeignKey("AdminId")]
    public virtual ProjectAdmin Admin { get; set; }

    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.InProgress;
    public virtual ICollection<User> AllUsers { get; set; } = new List<User>();
    public virtual ICollection<Desk> AllDesks { get; set; } = new List<Desk>();
}