using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Api.Models;

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

    public User()
    {
    }

    public User(string firstName, string lastName, string login, string email, string password, string? phone = null,
        byte[]? photo = null, UserStatus userStatus = UserStatus.User)
    {
        FirstName = firstName;
        LastName = lastName;
        Login = login;
        Email = email;
        Password = password;
        Phone = phone;
        Photo = photo;
        UserStatus = userStatus;
    }

    public User(UserModel user)
    {
        FirstName = user.FirstName;
        LastName = user.LastName;
        Login = user.Login;
        Email = user.Email;
        Password = user.Password;
        Phone = user.Phone;
        Photo = user.Photo;
        UserStatus = user.UserStatus;
    }

    public UserModel ToDto()
    {
        return new UserModel
        {
            Id = this.Id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Login = this.Login,
            Email = this.Email,
            Password = this.Password,
            Phone = this.Phone,
            Photo = this.Photo,
            UserStatus = this.UserStatus,
            RegistrationDate = this.RegistrationDate,
            LastLoginDate = this.LastLoginDate,
        };
    }
}