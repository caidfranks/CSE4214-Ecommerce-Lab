using Google.Cloud.Firestore;
using GameVault.Shared.Models;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class Invoice
{

  [FirestoreProperty]
  public required string Id { get; set; }

  [FirestoreProperty]
  public required int Total { get; set; }

  [FirestoreProperty]
  public required InvoiceStatus Status { get; set; }

  [FirestoreProperty]
  public required DateTime OrderDate { get; set; }

  [FirestoreProperty]
  public required DateTime ApprovedDate { get; set; }

  [FirestoreProperty]
  public required DateTime ShippedDate { get; set; }

  [FirestoreProperty]
  public required DateTime CompletedDate { get; set; }

  [FirestoreProperty]
  public required DateTime DeclinedDate { get; set; }

  [FirestoreProperty]
  public required DateTime CancelledDate { get; set; }

  [FirestoreProperty]
  public required DateTime ReturnRequestDate { get; set; }

  [FirestoreProperty]
  public required DateTime ReturnApprovedDate { get; set; }

  [FirestoreProperty]
  public required string CCInfo { get; set; }

  [FirestoreProperty]
  public required string ShipTo { get; set; }

  [FirestoreProperty]
  public required string OrderId { get; set; }

  [FirestoreProperty]
  public required string VendorId { get; set; }
}