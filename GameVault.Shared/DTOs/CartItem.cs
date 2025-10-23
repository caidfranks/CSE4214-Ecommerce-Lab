using System.Dynamic;

namespace GameVault.Shared.DTOs;

public class CartItemDto
{
    public string ListingId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ListingName { get; set; } = string.Empty;
    public int PriceInCents { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public int LineTotalInCents => PriceInCents * Quantity;


}