using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace GameVault.Server.Services;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _firebaseAuth;

    public FirebaseAuthService(IConfiguration configuration)
    {
        string emulatorHost = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST");
        if (!string.IsNullOrEmpty(emulatorHost))
        {
            // Handle the case where the emulator variable is not set
            // throw new InvalidOperationException("FIREBASE_AUTH_EMULATOR_HOST environment variable not set.");
            Console.WriteLine("Connecting to local auth emulator.");
        }
        var projectId = configuration["Firebase:ProjectId"] ?? throw new InvalidOperationException("Firebase ProjectId is required");
        var clientEmail = configuration["Firebase:ClientEmail"] ?? throw new InvalidOperationException("Firebase ClientEmail is required");
        var privateKey = configuration["Firebase:PrivateKey"] ?? throw new InvalidOperationException("Firebase PrivateKey is required");

        var credential = GoogleCredential.FromJson($@"{{
                ""type"": ""service_account"",
                ""project_id"": ""{projectId}"",
                ""client_email"": ""{clientEmail}"",
                ""private_key"": ""{privateKey.Replace("\\n", "\n")}""
            }}");

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential,
                ProjectId = projectId
            });
        }

        _firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    public async Task<string?> VerifyTokenAsync(string idToken)
    {
        var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
        return decodedToken.Uid;
    }

    public async Task<string?> CreateUserAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
            EmailVerified = false
        };

        var userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
        return userRecord.Uid;
    }

    public async Task<FirebaseUserInfo?> GetUserAsync(string userId)
    {
        var userRecord = await _firebaseAuth.GetUserAsync(userId);

        return new FirebaseUserInfo
        {
            UserId = userRecord.Uid,
            Email = userRecord.Email,
            EmailVerified = userRecord.EmailVerified,
            DisplayName = userRecord.DisplayName
        };
    }
}