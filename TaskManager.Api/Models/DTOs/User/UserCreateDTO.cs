using System.ComponentModel.DataAnnotations;
using TaskManager.Models;

namespace TaskManager.Api.Models.DTOs;

public class UserCreateDTO
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
        
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
        
    [Required]
    [StringLength(50)]
    public string Login { get; set; }
        
    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; }
        
    [Required]
    [StringLength(100)]
    [MinLength(3)]
    public string Password { get; set; }
    
    [Required]
    [Phone]
    public string Phone { get; set; }
        
    public byte[]? Photo { get; set; }
        
    public UserStatus UserStatus { get; set; } = UserStatus.User;
}