using Newtonsoft.Json;

namespace TaskManager.Client.Models;

public class AuthToken
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("expires_in")]
    public string ExpiresIn { get; set; }
}