using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Server.Services;
using GameVault.Server.Tests.Data;

namespace GameVault.Server.Tests.Stubs;

public class FirestoreServiceStub : IFirestoreService
{
  private bool Fail = false;
  private bool Empty = false;
  private CategoryExample categoryExample;

  public FirestoreServiceStub()
  {
    Console.WriteLine("Firestore stub initalized.");
    categoryExample = new();
  }

  public void SetFail(bool newFail)
  {
    Fail = newFail;
  }

  public void SetEmpty(bool newEmpty)
  {
    Empty = newEmpty;
  }

  public async Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class
  {
    // var docRef = _firestoreDb.Collection(collection).Document(documentId);
    // var snapshot = await docRef.GetSnapshotAsync();

    // if (!snapshot.Exists)
    // {
    //   return null;
    // }

    // return snapshot.ConvertTo<T>();
    Console.WriteLine("Got document.");

    T? result = null;
    return result;
  }

  public async Task<List<T>> GetCollectionAsync<T>(string collection) where T : class
  {
    // var collectionRef = _firestoreDb.Collection(collection);
    // var snapshot = await collectionRef.GetSnapshotAsync();

    // var results = new List<T>();
    // foreach (var document in snapshot.Documents)
    // {
    //   var obj = document.ConvertTo<T>();

    //   // If the object implements IHasId, populate the document ID
    //   if (obj is IHasId hasId)
    //   {
    //     hasId.Id = document.Id;
    //   }

    //   results.Add(obj);
    // }

    // return results;
    if (Fail) throw new Exception();
    Console.WriteLine("Got collection.");

    List<T> results = [];
    return results;
  }

  public async Task<List<T>> GetCollectionAsyncWithId<T>(string collection) where T : IHasId
  {
    // var collectionRef = _firestoreDb.Collection(collection);
    // var snapshot = await collectionRef.GetSnapshotAsync();

    // var results = new List<T>();
    // foreach (var document in snapshot.Documents)
    // {
    //   T newResult = document.ConvertTo<T>();
    //   newResult.Id = document.Id;
    //   results.Add(newResult);
    // }

    // return results;
    if (Fail) throw new Exception("Intentional fail");

    if (Empty)
    {
      return [];
    }

    Console.WriteLine("Got collection with id.");

    List<T> results = [];
    switch (typeof(T))
    {
      case Type t when t == typeof(Category):
        if (collection != "categories")
        {
          throw new Exception($"Tried to get Category from {collection}");
        }
        Console.WriteLine("Category");
        foreach (Category cat in categoryExample.categories)
        {
          results.Add((T)(object)cat);
        }
        break;
      default:
        Console.WriteLine("Other");
        break;
    }
    return results;
  }

  public async Task SetDocumentAsync(string collection, string documentId, object data)
  {
    // var docRef = _firestoreDb.Collection(collection).Document(documentId);
    // if (docRef == null)
    // {
    //   return;
    // }
    // await docRef.SetAsync(data, SetOptions.MergeAll);
    Console.WriteLine("Set document async.");
  }

  public async Task SetDocumentFieldAsync(string collection, string documentId, string fieldName, object value)
  {
    // var docRef = _firestoreDb.Collection(collection).Document(documentId);
    // await docRef.UpdateAsync(fieldName, value);
    Console.WriteLine("Set document field async.");
  }

  public async Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class
  {
    // DocumentReference docId = await _firestoreDb.Collection(collection).AddAsync(data);
    // return docId;
    throw new NotImplementedException();
  }

  public async Task DeleteDocumentAsync(string collection, string documentId)
  {
    // var docRef = _firestoreDb.Collection(collection).Document(documentId);
    // await docRef.DeleteAsync();
    Console.WriteLine("Deleted document.");
  }

  public async Task<List<T>> QueryDocumentsAsync<T>(string collection, string fieldName, object value) where T : class
  {
    // var collectionRef = _firestoreDb.Collection(collection);
    // var query = collectionRef.WhereEqualTo(fieldName, value);
    // var snapshot = await query.GetSnapshotAsync();

    // var results = new List<T>();
    // foreach (var document in snapshot.Documents)
    // {
    //   results.Add(document.ConvertTo<T>());
    // }

    // return results;
    Console.WriteLine("Queried documents.");

    List<T> results = [];
    return results;
  }

  public async Task<List<T>> QueryDocumentsAsyncWithId<T>(string collection, string fieldName, object value) where T : IHasId
  {
    // var collectionRef = _firestoreDb.Collection(collection);
    // var query = collectionRef.WhereEqualTo(fieldName, value);
    // var snapshot = await query.GetSnapshotAsync();

    // var results = new List<T>();
    // foreach (var document in snapshot.Documents)
    // {
    //   T newResult = document.ConvertTo<T>();
    //   newResult.Id = document.Id;
    //   results.Add(newResult);
    // }

    // return results;
    Console.WriteLine("Queried documents with id.");

    List<T> results = [];
    return results;
  }

  public async Task<List<T>> QueryComplexDocumentsAsync<T>(string collection, List<QueryParam> queries) where T : class
  {
    // if (queries.Count == 0) throw new ArgumentException("No queries provided");
    // var collectionRef = _firestoreDb.Collection(collection);
    // Query query = collectionRef.WhereEqualTo(queries[0].fieldName, queries[0].value);
    // // Console.WriteLine($"Querying where {queries[0].fieldName} = {queries[0].value}");
    // bool first = true;
    // foreach (QueryParam q in queries)
    // {
    //   if (!first)
    //   {
    //     query = query.WhereEqualTo(q.fieldName, q.value);
    //     // Console.WriteLine($"Querying where {q.fieldName} = {q.value}");
    //   }
    //   first = false;
    // }
    // var snapshot = await query.GetSnapshotAsync();

    // var results = new List<T>();
    // foreach (var document in snapshot.Documents)
    // {
    //   // T newResult = document.ConvertTo<T>();
    //   results.Add(document.ConvertTo<T>());
    // }

    // // Console.WriteLine($"Query returned {results.Count} results from {snapshot.Documents.Count} documents");

    // return results;
    Console.WriteLine("Queried complex documents.");

    List<T> results = [];
    return results;
  }

  public async Task<List<T>> QueryComplexDocumentsAsyncWithId<T>(string collection, List<QueryParam> queries) where T : IHasId
  {
    // if (queries.Count == 0) throw new ArgumentException("No queries provided");
    // var collectionRef = _firestoreDb.Collection(collection);
    // Query query = collectionRef.WhereEqualTo(queries[0].fieldName, queries[0].value);
    // // Console.WriteLine($"Querying where {queries[0].fieldName} = {queries[0].value}");
    // bool first = true;
    // foreach (QueryParam q in queries)
    // {
    //   if (!first)
    //   {
    //     query = query.WhereEqualTo(q.fieldName, q.value);
    //     // Console.WriteLine($"Querying where {q.fieldName} = {q.value}");
    //   }
    //   first = false;
    // }
    // var snapshot = await query.GetSnapshotAsync();

    // var results = new List<T>();
    // foreach (var document in snapshot.Documents)
    // {
    //   T newResult = document.ConvertTo<T>();
    //   newResult.Id = document.Id;
    //   results.Add(newResult);
    // }

    // // Console.WriteLine($"Query returned {results.Count} results from {snapshot.Documents.Count} documents");

    // return results;
    Console.WriteLine("Queried complex documents with id.");

    List<T> results = [];
    return results;
  }
}