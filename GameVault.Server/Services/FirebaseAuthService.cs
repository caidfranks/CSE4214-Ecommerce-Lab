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
        else
        {
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
    }

    public async Task<string?> VerifyTokenAsync(string idToken)
    {
        var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
        return decodedToken.Uid;
    }

    public async Task<string?> CreateUserAsync(string email, string password)
    {
        try
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
        catch (FirebaseAuthException ex)
        {
            // Handle specific Firebase Auth errors based on the error message
            if (ex.Message.Contains("EMAIL_EXISTS") || ex.Message.Contains("email already exists"))
            {
                throw new InvalidOperationException("An account with this email already exists. Please use a different email or try logging in instead.");
            }
            else if (ex.Message.Contains("INVALID_EMAIL") || ex.Message.Contains("invalid email"))
            {
                throw new InvalidOperationException("The email address is invalid. Please provide a valid email address.");
            }
            else if (ex.Message.Contains("WEAK_PASSWORD") || ex.Message.Contains("weak password"))
            {
                throw new InvalidOperationException("The password is too weak. Please choose a stronger password.");
            }
            else
            {
                throw new InvalidOperationException($"Authentication error: {ex.Message}");
            }
        }
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