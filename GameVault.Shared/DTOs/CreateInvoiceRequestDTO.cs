using GameVault.Shared.Models;
namespace GameVault.Shared.DTOs;

public class CreateInvoiceRequestDTO
{
    public required string OrderId { get; set; }
    public required string VendorId { get; set; }
    public required List<CartItemDTO> VendorItems { get; set; }
    public required Address ShipTo { get; set; }
    public required string PaymentId { get; set; }
}