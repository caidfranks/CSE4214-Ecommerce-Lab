using GameVault.Client.Models;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using System.Net.Http.Json;

namespace GameVault.Client.Services
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
                var response = await _httpClient.GetAsync("api/product");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching products: {response.StatusCode}");
                    return [];
                }

                var result = await response.Content.ReadFromJsonAsync<ListResponse<FullListingDTO>>();

                if (result is null || !result.Success)
                {
                    return [];
                }

                var listings = result.List;

                // Convert Listing to Product
                return listings!.Select(l => Product.FromFullListingDTO(l)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return [];
            }
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/product/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching product: {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<DataResponse<FullListingDTO>>();

                if (result is null || !result.Success)
                {
                    return null;
                }

                return Product.FromFullListingDTO(result.Data!);
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
                var response = await _httpClient.GetAsync($"api/product/category/{category}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching category products: {response.StatusCode}");
                    return [];
                }

                var result = await response.Content.ReadFromJsonAsync<ListResponse<FullListingDTO>>();

                if (result is null || !result.Success)
                {
                    return [];
                }

                var listings = result.List;

                // Convert Listing to Product
                return listings!.Select(l => Product.FromFullListingDTO(l)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category products: {ex.Message}");
                return [];
            }
        }

        public async Task<List<CategoryDTO>?> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/product/categories");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching categories: {response.StatusCode}");
                    return [];
                }

                var result = await response.Content.ReadFromJsonAsync<ListResponse<CategoryDTO>>();

                if (result is null || !result.Success)
                {
                    return [];
                }

                return result.List;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return [];
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            try
            {
                var searchRequest = new { query };
                var response = await _httpClient.PostAsJsonAsync("api/product/search", searchRequest);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error searching products: {response.StatusCode}");
                    return [];
                }

                var result = await response.Content.ReadFromJsonAsync<ListResponse<FullListingDTO>>();

                if (result is null || !result.Success)
                {
                    return [];
                }

                var listings = result.List;

                // Convert Listing to Product
                return listings!.Select(l => Product.FromFullListingDTO(l)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching products: {ex.Message}");
                return [];
            }
        }
    }
}