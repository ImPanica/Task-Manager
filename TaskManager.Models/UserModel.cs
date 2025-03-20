namespace TaskManager.Models;

public class UserModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public UserStatus UserStatus { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public byte[]? Photo { get; set; }

    public UserModel() { }
    
    public UserModel(string firstName, string lastName, string login, string email, string password, string? phone = null,
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
    
    public UserModel(UserModel user)
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
}