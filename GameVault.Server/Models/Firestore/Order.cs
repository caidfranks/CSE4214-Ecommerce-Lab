using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreOrder
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  [FirestoreProperty] public required DateTime OrderDate { get; set; }

  [FirestoreProperty] public required string CustomerId { get; set; }
  [FirestoreProperty] public required int SubtotalInCents { get; set; }
  [FirestoreProperty] public required int TaxInCents { get; set; }
  [FirestoreProperty] public required int TotalInCents { get; set; }

  [FirestoreProperty] public required Address ShipTo { get; set; }
}

[FirestoreData]
public class Order : FirestoreOrder, IHasId
{
  public required string Id { get; set; }
}