using GameVault.Shared.DTOs;

namespace GameVault.Client.Models
{
    public class Product
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string VendorName { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string Category { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public int Stock { get; set; }
        public string Description { get; set; } = "";

        public static Product FromFullListingDTO(FullListingDTO dto)
        {
            return new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price / 100M,
                VendorName = dto.VendorName,
                ThumbnailUrl = dto.Image,
                Category = dto.Category,
                CategoryName = dto.CategoryName,
                Stock = dto.Stock,
                Description = dto.Description,
                // Leaving out:
                // Status
                // LastModified
                // OwnerId
            };
        }
    }
}
