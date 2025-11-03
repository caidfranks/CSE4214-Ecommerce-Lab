using GameVault.Shared.Models;

namespace GameVault.Shared.DTOs;

public class UserDTO
{
  public required string Id { get; set; }
  public required AccountType Type { get; set; }
  public required string Email { get; set; }
  public bool? Banned { get; set; }
  public string? BanMsg { get; set; }
  public string? Name { get; set; }
  public string? ReviewedBy { get; set; }
  public int? BalanceInCents { get; set; }
}

public class UnbanUserDTO
{
  public required string Id { get; set; }
}

public class BanUserDTO : UnbanUserDTO
{
  public required string BanMsg { get; set; }
}
