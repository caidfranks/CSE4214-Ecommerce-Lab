using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class InvoiceItem
{
  // Unique Unrelated ID

  [FirestoreProperty] public required string InvoiceId { get; set; }

  [FirestoreProperty] public required string ListingId { get; set; }

  [FirestoreProperty] public required int Quantity { get; set; }

  [FirestoreProperty] public required int PriceAtOrder { get; set; }
  [FirestoreProperty] public required string NameAtOrder { get; set; } = string.Empty;
  [FirestoreProperty] public required string DescAtOrder { get; set; } = string.Empty;
}