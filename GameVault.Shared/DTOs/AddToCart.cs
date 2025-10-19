namespace GameVault.Shared.DTOs;

public class AddToCartDto
{
    public string ListingId { get; set; } = string.Empty;
    public string ListingName  { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; }  = string.Empty;
    public int PriceInCents { get; set; }
    public int Quantity { get; set; }
    public string VendorId { get; set; }  = String.Empty;
    public string VendorName { get; set; }  = String.Empty;
}