using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using System.Net.Http.Json;

namespace GameVault.Client.Services;

public class CheckoutService
{
    private readonly HttpClient _http;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(HttpClient http, ILogger<CheckoutService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<CheckoutResponseDTO> ProcessCheckoutAsync(CheckoutRequestDTO request)
    {
        try
        {
            _logger.LogInformation("Processing checkout");

            var response = await _http.PostAsJsonAsync("/api/checkout", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CheckoutResponseDTO>();
                return result ?? new CheckoutResponseDTO
                {
                    Success = false,
                    ErrorMessage = "Invalid response from server"
                };
            }
            else
            {
                var errorResult = await response.Content.ReadFromJsonAsync<CheckoutResponseDTO>();
                return errorResult ?? new CheckoutResponseDTO
                {
                    Success = false,
                    ErrorMessage = $"Checkout failed: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during checkout");
            return new CheckoutResponseDTO
            {
                Success = false,
                ErrorMessage = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<EstimateTaxResponseDTO?> EstimateTaxAsync(int subtotalInCents, string state)
    {
        try
        {
            var request = new EstimateTaxRequestDTO
            {
                SubtotalInCents = subtotalInCents,
                State = state
            };

            var response = await _http.PostAsJsonAsync("/api/checkout/estimate-tax", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EstimateTaxResponseDTO>();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating tax");
            return null;
        }
    }

    public async Task<OrderDTO?> GetOrderAsync(string orderId)
    {
        try
        {
            return await _http.GetFromJsonAsync<OrderDTO>($"/api/orders/{orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {orderId}", orderId);
            return null;
        }
    }

    public async Task<List<OrderDTO>> GetMyOrdersAsync()
    {
        try
        {
            var orders = await _http.GetFromJsonAsync<List<OrderDTO>>("/api/orders");
            return orders ?? new List<OrderDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return new List<OrderDTO>();
        }
    }

    public async Task<bool> CancelOrderAsync(string orderId)
    {
        try
        {
            var response = await _http.PostAsync($"/api/orders/{orderId}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {orderId}", orderId);
            return false;
        }
    }
}