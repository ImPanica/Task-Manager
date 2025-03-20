using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Models.Domains.Domains;

namespace TaskManager.Api.Models;

public class ProjectAdmin
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public ProjectAdmin()
    {
        
    }

    public ProjectAdmin(User user)
    {
        Id = user.Id;
        User = user;
    }
}