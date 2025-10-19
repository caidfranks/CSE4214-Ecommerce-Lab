using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class BankingInfo
{

  [FirestoreProperty]
  public required string OwnerId { get; set; }

  [FirestoreProperty]
  public required int RoutingNumber { get; set; }

  [FirestoreProperty]
  public required int AccountNumber { get; set; }
}