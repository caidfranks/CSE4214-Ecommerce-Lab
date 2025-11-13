using GameVault.Shared.DTOs;

namespace GameVault.Shared.Models;

public class BaseUser
{
  public required string Id { get; set; }
  public required AccountType Type { get; set; }
  public required string Email { get; set; }
}

public class Bannable : BaseUser
{
  public required bool Banned { get; set; }
  public required string BanMsg { get; set; } = string.Empty;
}

public class Customer : Bannable
{
  public static Customer FromUserDTO(UserDTO dto)
  {
    if (dto.Type != AccountType.Customer) throw new InvalidCastException("Cannot convert non-customer to customer");
    return new()
    {
      Id = dto.Id,
      Type = dto.Type,
      Email = dto.Email,
      Banned = (bool)dto.Banned!,
      BanMsg = dto.BanMsg!
    };
  }
};

public class Vendor : Bannable
{
  public required string Name { get; set; }
  public required string ReviewedBy { get; set; }
  public static Vendor FromUserDTO(UserDTO dto)
  {
    if (dto.Type != AccountType.Vendor) throw new InvalidCastException("Cannot convert non-vendor to vendor");
    return new()
    {
      Id = dto.Id,
      Type = dto.Type,
      Email = dto.Email,
      Banned = (bool)dto.Banned!,
      BanMsg = dto.BanMsg!,
      Name = dto.Name!,
      ReviewedBy = dto.ReviewedBy!
    };
  }
}

public class Admin : BaseUser
{
  public static Admin FromUserDTO(UserDTO dto)
  {
    if (dto.Type != AccountType.Vendor) throw new InvalidCastException("Cannot convert non-vendor to vendor");
    return new()
    {
      Id = dto.Id,
      Type = dto.Type,
      Email = dto.Email
    };
  }
};