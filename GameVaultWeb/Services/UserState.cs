namespace BlazorQuickStart.Services;

public class UserState
{
    public bool IsLoggedIn { get; set; }
    public string Role { get; set; } = "Customer";
    public string? Username { get; set; }
}
