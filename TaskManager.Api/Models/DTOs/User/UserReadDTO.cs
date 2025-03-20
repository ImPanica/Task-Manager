using TaskManager.Api.Models.Domains;
using TaskManager.Models;

namespace TaskManager.Api.Models.DTOs;

public class UserReadDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public UserStatus UserStatus { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public byte[] Photo { get; set; }
        
    // Вывод полного имени
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Преобразует User в UserReadDTO
    /// </summary>
    public static UserReadDTO MapToUserReadDTO(User user)
    {
        return new UserReadDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Login = user.Login,
            Email = user.Email,
            Phone = user.Phone,
            UserStatus = user.UserStatus,
            RegistrationDate = user.RegistrationDate,
            LastLoginDate = user.LastLoginDate,
            Photo = user.Photo
        };
    }
}