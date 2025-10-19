using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Category
{

  [FirestoreProperty]
  public required string Id { get; set; }

  [FirestoreProperty]
  public required string Name { get; set; }
}