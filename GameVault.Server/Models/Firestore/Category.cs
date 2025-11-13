using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreCategory
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty]
  public required string Name { get; set; }
}

[FirestoreData]
public class Category : FirestoreCategory, IHasId
{
  public required string Id { get; set; }
}