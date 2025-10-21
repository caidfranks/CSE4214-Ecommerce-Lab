using Google.Cloud.Firestore;

namespace GameVault.Server.Models.Firestore;

[FirestoreData]
public class FirestoreBankingInfo
{
  // File ID
  // [FirestoreProperty]
  // public required string OwnerId { get; set; }

  [FirestoreProperty]
  public required int RoutingNumber { get; set; }

  [FirestoreProperty]
  public required int AccountNumber { get; set; }
}

[FirestoreData]
public class BankingInfoWithId : FirestoreBankingInfo, IHasId
{
  public required string Id { get; set; }
}

public class BankingInfo : FirestoreBankingInfo
{
  public required string OwnerId { get; set; }

  public static BankingInfo FromBankingInfoWithId(BankingInfoWithId oldBI)
  {
    return new()
    {
      OwnerId = oldBI.Id,
      RoutingNumber = oldBI.RoutingNumber,
      AccountNumber = oldBI.AccountNumber
    };
  }
}