using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreInvoice
{
  // File ID
  // [FirestoreProperty]
  // public required string Id { get; set; }

  // using subtotal as tax is not sent to vendor
  // [FirestoreProperty]
  // public required int Total { get; set; }

  [FirestoreProperty]
  public required InvoiceStatus Status { get; set; }

  [FirestoreProperty]
  public required DateTime OrderDate { get; set; }

  [FirestoreProperty]
  public DateTime? ApprovedDate { get; set; }

  [FirestoreProperty]
  public DateTime? ShippedDate { get; set; }

  [FirestoreProperty]
  public DateTime? CompletedDate { get; set; }

  [FirestoreProperty]
  public DateTime? DeclinedDate { get; set; }

  [FirestoreProperty]
  public DateTime? CancelledDate { get; set; }

  [FirestoreProperty]
  public DateTime? ReturnRequestDate { get; set; }

  [FirestoreProperty]
  public DateTime? ReturnApprovedDate { get; set; }

  // we shouldn't be storing CC info in Firestore
  // [FirestoreProperty]
  // public required string CCInfo { get; set; }

  //instead we use a token from Square
  [FirestoreProperty] 
  public required string PaymentId { get; set; }
  [FirestoreProperty]
  public required int SubtotalInCents { get; set; }
  
  [FirestoreProperty]
  public required Address ShipTo { get; set; }

  [FirestoreProperty]
  public required string OrderId { get; set; }

  [FirestoreProperty]
  public required string VendorId { get; set; }

  [FirestoreProperty]
  public string ReturnMsg { get; set; } = string.Empty;
}

[FirestoreData]
public class Invoice : FirestoreInvoice, IHasId
{
  public required string Id { get; set; }
}