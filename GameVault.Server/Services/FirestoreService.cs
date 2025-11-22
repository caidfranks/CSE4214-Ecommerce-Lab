using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;

namespace GameVault.Server.Services;

public class FirestoreService : IFirestoreService
{
    private readonly FirestoreDb _firestoreDb;

    public FirestoreService(IConfiguration configuration)
    {
        string? emulatorHost = Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_HOST");
        
        if (!string.IsNullOrEmpty(emulatorHost))
        {
            Console.WriteLine("Connecting to local firestore emulator.");
            var projectId = configuration["Firebase:ProjectId"] ?? "demo-project";

            var firestoreClientBuilder = new FirestoreClientBuilder
            {
                Endpoint = emulatorHost,
                ChannelCredentials = ChannelCredentials.Insecure
            };

            _firestoreDb = FirestoreDb.Create(projectId, firestoreClientBuilder.Build());
        }
        else
        {
            string? credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            GoogleCredential credential;
            string projectId;

            if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
            {
                Console.WriteLine($"Using Firestore credentials from file: {credentialsPath}");
                credential = GoogleCredential.FromFile(credentialsPath);
                
                var credentialsJson = System.Text.Json.JsonDocument.Parse(File.ReadAllText(credentialsPath));
                projectId = credentialsJson.RootElement.GetProperty("project_id").GetString() 
                    ?? throw new InvalidOperationException("Firebase ProjectId not found in credentials file");
            }
            else if (configuration["Firebase:ClientEmail"] != null && configuration["Firebase:PrivateKey"] != null)
            {
                Console.WriteLine("Using Firestore credentials from appsettings.json");
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
                Console.WriteLine("Using Firestore Application Default Credentials (Cloud Run)");
                projectId = configuration["Firebase:ProjectId"] ?? "gamevault-9a27e";
                
                try
                {
                    credential = GoogleCredential.GetApplicationDefault();
                    Console.WriteLine("Successfully loaded Firestore Application Default Credentials");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load Firestore Application Default Credentials: {ex.Message}");
                    throw new InvalidOperationException("Unable to load Firestore credentials. Please ensure the service is running on Google Cloud with proper permissions.", ex);
                }
            }

            var firestoreClientBuilder = new FirestoreClientBuilder
            {
                ChannelCredentials = credential.ToChannelCredentials()
            };

            _firestoreDb = FirestoreDb.Create(projectId, firestoreClientBuilder.Build());
        }
    }

    public async Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class
    {
        var docRef = _firestoreDb.Collection(collection).Document(documentId);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return null;
        }

        return snapshot.ConvertTo<T>();
    }

    public async Task<List<T>> GetCollectionAsync<T>(string collection) where T : class
    {
        var collectionRef = _firestoreDb.Collection(collection);
        var snapshot = await collectionRef.GetSnapshotAsync();

        var results = new List<T>();
        foreach (var document in snapshot.Documents)
        {
            var obj = document.ConvertTo<T>();

            // If the object implements IHasId, populate the document ID
            if (obj is IHasId hasId)
            {
                hasId.Id = document.Id;
            }

            results.Add(obj);
        }

        return results;
    }

    public async Task<List<T>> GetCollectionAsyncWithId<T>(string collection) where T : IHasId
    {
        var collectionRef = _firestoreDb.Collection(collection);
        var snapshot = await collectionRef.GetSnapshotAsync();

        var results = new List<T>();
        foreach (var document in snapshot.Documents)
        {
            T newResult = document.ConvertTo<T>();
            newResult.Id = document.Id;
            results.Add(newResult);
        }

        return results;
    }

    public async Task SetDocumentAsync(string collection, string documentId, object data)
    {
        var docRef = _firestoreDb.Collection(collection).Document(documentId);
        if (docRef == null)
        {
            return;
        }
        await docRef.SetAsync(data, SetOptions.MergeAll);
    }

    public async Task SetDocumentFieldAsync(string collection, string documentId, string fieldName, object value)
    {
        var docRef = _firestoreDb.Collection(collection).Document(documentId);
        await docRef.UpdateAsync(fieldName, value);
    }

    public async Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class
    {
        DocumentReference docId = await _firestoreDb.Collection(collection).AddAsync(data);
        return docId;
    }

    public async Task DeleteDocumentAsync(string collection, string documentId)
    {
        var docRef = _firestoreDb.Collection(collection).Document(documentId);
        await docRef.DeleteAsync();
    }

    public async Task<List<T>> QueryDocumentsAsync<T>(string collection, string fieldName, object value) where T : class
    {
        var collectionRef = _firestoreDb.Collection(collection);
        var query = collectionRef.WhereEqualTo(fieldName, value);
        var snapshot = await query.GetSnapshotAsync();

        var results = new List<T>();
        foreach (var document in snapshot.Documents)
        {
            results.Add(document.ConvertTo<T>());
        }

        return results;
    }

    public async Task<List<T>> QueryDocumentsAsyncWithId<T>(string collection, string fieldName, object value) where T : IHasId
    {
        var collectionRef = _firestoreDb.Collection(collection);
        var query = collectionRef.WhereEqualTo(fieldName, value);
        var snapshot = await query.GetSnapshotAsync();

        var results = new List<T>();
        foreach (var document in snapshot.Documents)
        {
            T newResult = document.ConvertTo<T>();
            newResult.Id = document.Id;
            results.Add(newResult);
        }

        return results;
    }

    public async Task<List<T>> QueryComplexDocumentsAsync<T>(string collection, List<QueryParam> queries) where T : class
    {
        if (queries.Count == 0) throw new ArgumentException("No queries provided");
        var collectionRef = _firestoreDb.Collection(collection);
        Query query = collectionRef.WhereEqualTo(queries[0].fieldName, queries[0].value);
        bool first = true;
        foreach (QueryParam q in queries)
        {
            if (!first)
            {
                query = query.WhereEqualTo(q.fieldName, q.value);
            }
            first = false;
        }
        var snapshot = await query.GetSnapshotAsync();

        var results = new List<T>();
        foreach (var document in snapshot.Documents)
        {
            results.Add(document.ConvertTo<T>());
        }

        return results;
    }

    public async Task<List<T>> QueryComplexDocumentsAsyncWithId<T>(string collection, List<QueryParam> queries) where T : IHasId
    {
        if (queries.Count == 0) throw new ArgumentException("No queries provided");
        var collectionRef = _firestoreDb.Collection(collection);
        Query query = collectionRef.WhereEqualTo(queries[0].fieldName, queries[0].value);
        bool first = true;
        foreach (QueryParam q in queries)
        {
            if (!first)
            {
                query = query.WhereEqualTo(q.fieldName, q.value);
            }
            first = false;
        }
        var snapshot = await query.GetSnapshotAsync();

        var results = new List<T>();
        foreach (var document in snapshot.Documents)
        {
            T newResult = document.ConvertTo<T>();
            newResult.Id = document.Id;
            results.Add(newResult);
        }

        return results;
    }
}
