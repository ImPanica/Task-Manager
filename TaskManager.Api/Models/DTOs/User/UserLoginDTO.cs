using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models.DTOs;

public class UserLoginDTO
{
    [Required]
    public string Login { get; set; }
        
    [Required]
    public string Password { get; set; }
}