using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class CartItem
{

  [FirestoreProperty]
  public required string CustomerId { get; set; }

  [FirestoreProperty]
  public required string ListingId { get; set; }

  [FirestoreProperty]
  public required int Quantity { get; set; }
}