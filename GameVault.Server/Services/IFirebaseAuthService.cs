namespace GameVault.Server.Services;
    

public interface IFirebaseAuthService
{
    Task<string?> VerifyTokenAsync(string idToken);
    Task<string?> CreateUserAsync(string email, string password);
    Task<FirebaseUserInfo?> GetUserAsync(string email);
}

public class FirebaseUserInfo
{
    public string UserId { get; set; } =  string.Empty;
    public string Email { get; set; } =  string.Empty;
    public bool EmailVerified { get; set; }
    public string? DisplayName { get; set; }
}