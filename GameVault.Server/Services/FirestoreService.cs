using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameVault.Server.Services;

public class FirestoreService : IFirestoreService
{
    private readonly FirestoreDb _firestoreDb;

    public FirestoreService(IConfiguration configuration)
    {


        string emulatorHost = Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_HOST");
        if (string.IsNullOrEmpty(emulatorHost))
        {
            // Handle the case where the emulator variable is not set
            // throw new InvalidOperationException("FIRESTORE_EMULATOR_HOST environment variable not set.");
            var projectId = configuration["Firebase:ProjectId"];
            var clientEmail = configuration["Firebase:ClientEmail"];
            var privateKey = configuration["Firebase:PrivateKey"];

            var credential = GoogleCredential.FromJson($@"{{
                ""type"": ""service_account"",
                ""project_id"": ""{projectId}"",
                ""client_email"": ""{clientEmail}"",
                ""private_key"": ""{privateKey?.Replace("\\n", "\n")}""
            }}");

            var firestoreClientBuilder = new FirestoreClientBuilder
            {
                ChannelCredentials = credential.ToChannelCredentials()
            };

            _firestoreDb = FirestoreDb.Create(projectId, firestoreClientBuilder.Build());
        }
        else
        {
            Console.WriteLine("Connecting to local firestore emulator.");

            var projectId = configuration["Firebase:ProjectId"];

            // Manually configure the FirestoreClientBuilder
            var firestoreClientBuilder = new FirestoreClientBuilder
            {
                Endpoint = emulatorHost,
                ChannelCredentials = ChannelCredentials.Insecure
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

            // 🔧 Attach the document ID if the model has an Id property
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null && idProp.CanWrite)
            {
                idProp.SetValue(obj, document.Id);
            }

            results.Add(obj);
        }

        return results;
    }


    public async Task SetDocumentAsync<T>(string collection, string documentId, T data) where T : class
    {
        var docRef = _firestoreDb.Collection(collection).Document(documentId);
        await docRef.SetAsync(data, SetOptions.MergeAll);
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

    public async Task<List<T>> QueryDocumentsAsync<T>(string collectionName, string field, object value)
    where T : class
    {
        try
        {
            var query = _firestoreDb.Collection(collectionName).WhereEqualTo(field, value);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot == null || snapshot.Count == 0)
            {
                return new List<T>();
            }

            return snapshot.Documents.Select(d => d.ConvertTo<T>()).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error querying documents in {collectionName}: {ex.Message}");
            return new List<T>();
        }
    }



}