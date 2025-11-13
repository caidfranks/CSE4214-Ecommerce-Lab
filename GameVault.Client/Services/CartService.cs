using System.Net.Http.Json;
using System.Text.Json;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace GameVault.Client.Services;

public class CartService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CartService> _logger;

    public CartService(HttpClient httpClient, ILogger<CartService> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<bool> AddToCartAsync(string listingId, int quantity, EventCallback onCartChanged)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/cart/item", new NewCartItemDTO() { ListingId = listingId, Quantity = quantity });

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("added to cart {listingId}", listingId);
                await onCartChanged.InvokeAsync();
                return true;
            }
            else
            {
                _logger.LogError("failed to add item to cart {listingId}", listingId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return false;
        }
    }

    public async Task<int> FetchCartItemCount()
    {
        try
        {
            var cart = await _httpClient.GetFromJsonAsync<CartDTO>("/api/cart");
            return cart?.TotalItemCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cart item count failed to load.");
            return 0;
        }
    }

    public async Task RemoveFromCartAsync(CartItemDTO Item, EventCallback OnCartChanged)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/cart/item/{Item.ListingId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("removed cart item");
                await OnCartChanged.InvokeAsync();
            }
            else
            {
                _logger.LogError("removal failed {Item.ListingId}", Item.ListingId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "removal error");
        }
    }

    public async Task UpdateQuantityAsync(int newQuantity, CartItemDTO item, EventCallback onCartChanged)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/cart/item/{item.ListingId}/quantity", new { Quantity = newQuantity });

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("qty updated {item.ListingId}", item.ListingId);
                item.Quantity = newQuantity;
                await onCartChanged.InvokeAsync();
            }
            else
            {
                _logger.LogError("qty update failed {item.ListingId}", item.ListingId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "update error");
        }
    }

    public async Task<CartDTO?> LoadCartAsync()
    {
        try
        {
            var cart = await _httpClient.GetFromJsonAsync<CartDTO>("/api/cart");
            _logger.LogInformation("Cart loaded successfully");
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cart");
            return null;
        }
    }

}