using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace GameVault.Server.Services;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _firebaseAuth;

    public FirebaseAuthService(IConfiguration configuration)
    {
        string emulatorHost = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST") ?? "";
        if (!string.IsNullOrEmpty(emulatorHost))
        {
            Console.WriteLine("Connecting to local auth emulator.");
        }

        GoogleCredential credential;
        string projectId;

        string? credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        
        if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
        {
            Console.WriteLine($"Using Firebase credentials from file: {credentialsPath}");
            credential = GoogleCredential.FromFile(credentialsPath);
            
            var credentialsJson = System.Text.Json.JsonDocument.Parse(File.ReadAllText(credentialsPath));
            projectId = credentialsJson.RootElement.GetProperty("project_id").GetString() 
                ?? throw new InvalidOperationException("Firebase ProjectId not found in credentials file");
        }
        else if (configuration["Firebase:ClientEmail"] != null && configuration["Firebase:PrivateKey"] != null)
        {
            Console.WriteLine("Using Firebase credentials from appsettings.json");
            projectId = configuration["Firebase:ProjectId"] ?? throw new InvalidOperationException("Firebase ProjectId is required");
            var clientEmail = configuration["Firebase:ClientEmail"];
            var privateKey = configuration["Firebase:PrivateKey"];

            credential = GoogleCredential.FromJson($@"{{
                ""type"": ""service_account"",
                ""project_id"": ""{projectId}"",
                ""client_email"": ""{clientEmail}"",
                ""private_key"": ""{privateKey.Replace("\\n", "\n")}""
            }}");
        }
        else
        {
            Console.WriteLine("Using Application Default Credentials (Cloud Run)");
            projectId = configuration["Firebase:ProjectId"] ?? "gamevault-9a27e";
            
            try
            {
                credential = GoogleCredential.GetApplicationDefault();
                Console.WriteLine("Successfully loaded Application Default Credentials");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load Application Default Credentials: {ex.Message}");
                throw new InvalidOperationException("Unable to load Firebase credentials. Please ensure the service is running on Google Cloud with proper permissions.", ex);
            }
        }

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
        return decodedToken?.Uid ?? null;
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
