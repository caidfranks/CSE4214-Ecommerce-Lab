using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Tag
{

  [FirestoreProperty]
  public required string CategoryId { get; set; }

  [FirestoreProperty]
  public required string ListingId { get; set; }
}