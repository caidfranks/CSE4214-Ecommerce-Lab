using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Cart
{

  [FirestoreProperty]
  public required string OwnerId { get; set; }
}