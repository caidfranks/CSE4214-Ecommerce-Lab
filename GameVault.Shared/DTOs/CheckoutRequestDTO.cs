using GameVault.Shared.Models;

namespace GameVault.Shared.DTOs;

public class CheckoutRequestDTO
{
    public required Address ShipTo { get; set; }
    public required string PaymentMethodId { get; set; }
}
