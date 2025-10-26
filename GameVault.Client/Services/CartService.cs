using System.Net.Http.Json;
using System.Text.Json;
using GameVault.Client.Components;
using GameVault.Client.Pages;
using GameVault.Shared.DTOs;
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

    public async Task<bool> AddToCartAsync(int listingId, EventCallback onCartChanged)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/cart/item", new { ListingId = listingId, Quantity = 1 });

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("added to cart", listingId);
                await onCartChanged.InvokeAsync();
                return true;
            }
            else
            {
                _logger.LogError("failed to add item to cart", listingId);
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
            var cart = await _httpClient.GetFromJsonAsync<ShoppingCartDto>("/api/cart");
            return cart?.TotalItemCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cart item count failed to load.");
            return 0;
        }
    }

    public async Task RemoveFromCartAsync(CartItemDto Item, EventCallback OnCartChanged)
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
                _logger.LogError("removal failed", Item.ListingId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "removal error");
        }
    }

    public async Task UpdateQuantityAsync(int newQuantity, CartItemDto item, EventCallback onCartChanged)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/cart/item/{item.ListingId}/quantity", new { Quantity = newQuantity });

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("qty updated", item.ListingId);
                item.Quantity = newQuantity;
                await onCartChanged.InvokeAsync();
            }
            else
            {
                _logger.LogError("qty update failed", item.ListingId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "update error");
        }
    }

    public async Task<ShoppingCartDto?> LoadCartAsync()
    {
        try
        {
            var cart = await _httpClient.GetFromJsonAsync<ShoppingCartDto>("/api/cart");
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