using GameVaultWeb.Models;
using System.Net.Http.Json;

namespace GameVaultWeb.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching products: {response.StatusCode}");
                    return new List<Product>();
                }

                var listings = await response.Content.ReadFromJsonAsync<List<Listing>>();

                if (listings == null)
                {
                    return new List<Product>();
                }

                // Convert Listing to Product
                return listings.Select(l => ConvertListingToProduct(l)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching product: {response.StatusCode}");
                    return null;
                }

                var listing = await response.Content.ReadFromJsonAsync<Listing>();

                if (listing == null)
                {
                    return null;
                }

                return ConvertListingToProduct(listing);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/category/{category}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching category products: {response.StatusCode}");
                    return new List<Product>();
                }

                var listings = await response.Content.ReadFromJsonAsync<List<Listing>>();

                if (listings == null)
                {
                    return new List<Product>();
                }

                return listings.Select(l => ConvertListingToProduct(l)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category products: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            try
            {
                var searchRequest = new { query };
                var response = await _httpClient.PostAsJsonAsync("api/products/search", searchRequest);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error searching products: {response.StatusCode}");
                    return new List<Product>();
                }

                var listings = await response.Content.ReadFromJsonAsync<List<Listing>>();

                if (listings == null)
                {
                    return new List<Product>();
                }

                return listings.Select(l => ConvertListingToProduct(l)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching products: {ex.Message}");
                return new List<Product>();
            }
        }

        private Product ConvertListingToProduct(Listing listing)
        {
            return new Product
            {
                Id = listing.Id,
                Name = listing.Name,
                Description = listing.Description,
                Price = listing.Price / 100m, // Convert cents to dollars
                Stock = listing.Stock,
                Category = listing.Category,
                VendorName = listing.OwnerID,
                ThumbnailUrl = listing.Image,
            };
        }
    }

    // Firestore Listing model (matches your database structure)
    public class Listing
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string OwnerID { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}