using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Shared.DTOs;

public class NewAccountDTO
{
  public required AccountStatus ApprovalStatus { get; set; }
  //public string ApprovedAt { get; set; } = string.Empty;
  public string ApprovedBy { get; set; } = string.Empty;
  public required string BusinessName { get; set; }
  //public string CreatedAt { get; set; } = string.Empty;
  public required string DisplayName { get; set; }
  public required string Email { get; set; }
  public string RejectionReason { get; set; } = string.Empty;
  public required string Role { get; set; }
  //public string UpdatedAt { get; set; } = string.Empty;
}
public class AccountDTO : NewAccountDTO
{
  public required string UserID { get; set; }
}

public class UpdateAccountDTO
{
    public string? DisplayName { get; set; }
}
