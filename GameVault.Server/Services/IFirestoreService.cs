using Google.Cloud.Firestore;

namespace GameVault.Server.Services;

public interface IFirestoreService
{
    Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class;
    Task<List<T>> GetCollectionAsync<T>(string collection) where T : class;
    Task SetDocumentAsync<T>(string collection, string documentId, T data) where T : class;
    Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class;
    Task DeleteDocumentAsync(string collection, string documentId);
    Task<List<T>> QueryDocumentsAsync<T>(string collection, string fieldName, object value) where T : class;
}