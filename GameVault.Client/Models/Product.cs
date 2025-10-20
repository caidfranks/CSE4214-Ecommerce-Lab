namespace GameVaultWeb.Models
{
    public class Product
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string VendorName { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string Category { get; set; } = "";
        public int Stock { get; set; }
        public string Description { get; set; } = "";
    }
}
