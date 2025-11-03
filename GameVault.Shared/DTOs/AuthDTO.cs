using GameVault.Shared.Models;

namespace GameVault.Shared.DTOs;

public class AuthResponse : DataResponse<UserDTO>
{
  public string? IdToken { get; set; }
}

public class RegisterCustomerRequest
{
  public required string Email { get; set; }
  public required string Password { get; set; }
}

public class RegisterAdminRequest : RegisterCustomerRequest
{
    // public required string DisplayName { get; set; }
}

public class RegisterVendorRequest : RegisterCustomerRequest
{
    public required string Id { get; set; }
  public required string DisplayName { get; set; }
  public required string Reason { get; set; }
}

public class RequestDTO
{
  public required string Id { get; set; }
  public required string Email { get; set; }
  public required string Password { get; set; }
  public required string Name { get; set; }
  public required string Reason { get; set; }
  public required DateTime Timestamp { get; set; }
    public bool Archived { get; set; }
}