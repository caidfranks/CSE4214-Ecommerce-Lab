using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Order
{

  [FirestoreProperty]
  public required string Id { get; set; }

  [FirestoreProperty]
  public required DateTime OrderDate { get; set; }

  [FirestoreProperty]
  public required string CustomerId { get; set; }
}