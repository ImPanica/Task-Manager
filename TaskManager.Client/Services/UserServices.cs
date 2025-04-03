using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using TaskManager.Api.Models.DTOs;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services;

public class UserServices
{
    public async Task<AuthToken> Login(string userName, string password)
    {
        string url = "http://localhost:5054/api/account/login";
        var userDto = new UserLoginDTO
        {
            Login = userName,
            Password = password
        };
        var jsonDto = JsonConvert.SerializeObject(userDto);
        var content = new StringContent(jsonDto, Encoding.UTF8, "application/json");
        using (var client = new HttpClient())
        {
            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<AuthToken>(json);
            return auth;
        }
    }
}