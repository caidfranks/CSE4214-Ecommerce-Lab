namespace GameVault.Shared.Models
{
    public class CartItem
    {
        public string CartItemId { get; set; }
        public string ListingId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ListingName { get; set; }
        public int PriceAtAddTimeInCents { get; set; }
        public int Quantity { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime AddedAt { get; set; }

        public CartItem()
        {
        }

        public CartItem(string listingId, string thumbnailUrl, string listingName, int priceInCents, int quantity, string vendorId, string vendorName)
        {
            ListingId = listingId;
            ThumbnailUrl = thumbnailUrl;
            ListingName = listingName;
            PriceAtAddTimeInCents = priceInCents;
            Quantity = quantity;
            VendorId = vendorId;
            VendorName = vendorName;
            AddedAt = DateTime.UtcNow;
            
        }

    }
    
}