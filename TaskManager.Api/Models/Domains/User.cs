using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TaskManager.Api.Models.Domains.Domains;
using TaskManager.Models;

namespace TaskManager.Api.Models.Domains;

public class User : IdentityUser
{
    [Key] public int Id { get; set; }
    [Required] [StringLength(50)] public string FirstName { get; set; }
    [Required] [StringLength(50)] public string LastName { get; set; }
    [Required] [StringLength(50)] public string Login { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; }

    [Required] [StringLength(256)] public string Password { get; set; }
    [Phone] public string Phone { get; set; }
    public UserStatus UserStatus { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public DateTime LastLoginDate { get; set; }
    public byte[]? Photo { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<Desk> Desks { get; set; } = new List<Desk>();
    public virtual ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
    public virtual ICollection<Task> ExecutedTasks { get; set; } = new List<Task>();
    public virtual ICollection<ProjectAdmin> AdminProjects { get; set; } = new List<ProjectAdmin>();
}