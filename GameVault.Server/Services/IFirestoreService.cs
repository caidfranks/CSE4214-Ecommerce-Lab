using Google.Cloud.Firestore;
using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;

namespace GameVault.Server.Services;

public interface IFirestoreService
{
    Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class;
    Task<List<T>> GetCollectionAsync<T>(string collection) where T : class;
    Task<List<T>> GetCollectionAsyncWithId<T>(string collection) where T : IHasId;
    Task SetDocumentAsync(string collection, string documentId, object data);
    Task SetDocumentFieldAsync(string collection, string documentId, string fieldName, object value);
    Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class;
    Task DeleteDocumentAsync(string collection, string documentId);
    Task<List<T>> QueryDocumentsAsync<T>(string collection, string fieldName, object value) where T : class;
    Task<List<T>> QueryDocumentsAsyncWithId<T>(string collection, string fieldName, object value) where T : IHasId;
    Task<List<T>> QueryComplexDocumentsAsync<T>(string collection, List<QueryParam> queries) where T : class;
    Task<List<T>> QueryComplexDocumentsAsyncWithId<T>(string collection, List<QueryParam> queries) where T : IHasId;
}